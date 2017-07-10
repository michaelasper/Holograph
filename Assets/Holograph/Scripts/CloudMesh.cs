using System;
using UnityEngine;

public class CloudMesh : MonoBehaviour
{
    static Material mat;
    public float scale;
    private Transform cam;
    public Color triangleColor;
    public int vertexNum;
    public int triangleNum;
    public float maxSpinSpeed;
    public float minLifeSpan;
    public float maxLifeSpan;
    public Color bottomGlow;
    public Color topGlow;
    public Vector3 glowAxis;
    private Vector3[] vertices;
    private Vector3[] trianglePoints;
    private int[][] closestPoints;
    private float[] spinningSpeed;
    private float[] age;
    private float[] lifeSpan;

    void Start()
    {
        cam = Camera.main.transform;
        initVertices();
        closestPoints = new int[triangleNum][];
        lifeSpan = new float[triangleNum];
        age = new float[triangleNum];
        for (int i = 0; i < triangleNum; ++i)
        {
            updateClosestPointsForTriangle(i);
            lifeSpan[i] = UnityEngine.Random.Range(minLifeSpan, maxLifeSpan);
            age[i] = UnityEngine.Random.Range(0f, lifeSpan[i]);
        }
        spinningSpeed = new float[vertexNum];
        for (int i = 0; i < vertexNum; ++i)
        {
            spinningSpeed[i] = UnityEngine.Random.Range(-1f, 1f);
        }
    }

    void Update()
    {
        CreateMaterial();
    }

    private void initVertices()
    {
        vertices = new Vector3[vertexNum];
        for (int i = 0; i < vertexNum; ++i)
        {
            Vector3 vertex;
            do
            {
                vertex = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            }
            while (vertex.sqrMagnitude > 1f);
            vertices[i] = vertex.normalized * scale;
            vertices[i].y *= .4f;
        }
        trianglePoints = new Vector3[triangleNum];
        for (int i = 0; i < triangleNum; ++i)
        {
            updateTrianglePoint(i);
        }
    }

    private void updateTrianglePoint(int i)
    {
        Vector3 trianglePoint;
        do
        {
            trianglePoint = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
        }
        while (trianglePoint.sqrMagnitude > 1f);
        trianglePoints[i] = trianglePoint.normalized * scale;
        trianglePoints[i].y *= .4f;
    }

    private void updateClosestPointsForTriangle(int i)
    {
        closestPoints[i] = new int[3];
        float[] smallestDistances = new float[3];
        float[] distances = new float[vertexNum];
        for (int j = 0; j < vertexNum; ++j)
        {
            distances[j] = Vector3.Distance(vertices[j], trianglePoints[i]);
        }
        for (int j = 0; j < 3; ++j)
        {
            smallestDistances[j] = 1000f;
        }
        for (int j = 0; j < vertexNum; ++j)
        {
            for (int k = 0; k < 3; ++k)
            {
                if (distances[j] < smallestDistances[k])
                {
                    for (int p = 2; p > k; --p)
                    {
                        smallestDistances[p] = smallestDistances[p - 1];
                        closestPoints[i][p] = closestPoints[i][p - 1];
                    }
                    smallestDistances[k] = distances[j];
                    closestPoints[i][k] = j;
                    break;
                }
            }
        }
    }

    static void CreateMaterial()
    {
        if (!mat)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            mat.SetInt("_ZWrite", 0);
        }
    }

    public void OnRenderObject()
    {
        float[] rimlight = new float[triangleNum];
        GL.PushMatrix();
        mat.SetPass(0);
        GL.Begin(GL.TRIANGLES);
        for (int i = 0; i < triangleNum; ++i)
        {
            if (age[i] > lifeSpan[i])
            {
                age[i] = 0;
                updateTrianglePoint(i);
                updateClosestPointsForTriangle(i);
            }
            float brightness = 1f - age[i] / lifeSpan[i];
            age[i] += Time.deltaTime;
            Vector3 normal = Vector3.Cross(vertices[closestPoints[i][1]] - vertices[closestPoints[i][0]], vertices[closestPoints[i][2]] - vertices[closestPoints[i][0]]).normalized;
            Vector3 center = (vertices[closestPoints[i][0]] + vertices[closestPoints[i][1]] + vertices[closestPoints[i][2]]) / 3f;
            Vector3 viewingDir = center - cam.position;
            rimlight[i] = Mathf.Pow(1f - Mathf.Abs(Vector3.Angle(normal, viewingDir) / 90f - 1f), 2);
            float glowAngle = Vector3.Angle(center, glowAxis);
            float topColorAmt = glowAngle < 90f ? 1f - glowAngle / 90f : 0f;
            float bottomColorAmt = glowAngle > 90f ? glowAngle / 90f - 1f : 0f;
            GL.Color((triangleColor + bottomGlow * bottomColorAmt + topGlow * topColorAmt) * rimlight[i] * brightness);
            for (int j = 0; j < 3; ++j)
            {
                GL.Vertex(vertices[closestPoints[i][j]]);
            }
        }
        GL.End();
        GL.Begin(GL.LINES);
        for (int i = 0; i < triangleNum; ++i)
        {
            GL.Color(triangleColor * rimlight[i]);
            for (int j = 0; j < 3; ++j)
            {
                GL.Vertex(vertices[closestPoints[i][j]]);
                GL.Vertex(vertices[closestPoints[i][(j + 1) % 3]]);
            }
        }
        GL.End();
        GL.PopMatrix();
        for (int i = 0; i < vertexNum; ++i)
        {
            vertices[i] = Quaternion.AngleAxis(spinningSpeed[i] * Time.deltaTime * maxSpinSpeed, new Vector3(0, 1, 0)) * vertices[i];
        }
    }
}