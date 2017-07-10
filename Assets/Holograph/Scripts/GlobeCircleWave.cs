using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeCircleWave : MonoBehaviour
{

    public bool showCircle;
    public float circleRadius;

    private float deltaAngle = Mathf.PI / 10f;
    private Vector2[] circleLines;
    private Vector3 circleBaseX;
    private Vector3 circleBaseY;
    private Vector3 circleCenter;
    private static Material lineMaterial;

    // Use this for initialization
    void Start()
    {
        showCircle = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    public void OnRenderObject()
    {
        if (circleCenter != Vector3.zero)
        {
            //Debug.Log("Drawing circle");
            CreateLineMaterial();
            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            lineMaterial.SetPass(0);
            GL.Begin(GL.LINES);
            float a = 1f - circleRadius / 0.07f;
            GL.Color(new Color(a, a, a, 1f));
            Vector3? v0 = null;
            foreach (Vector2 v in circleLines)
            {
                //Debug.Log("drew a line");
                if (v0.HasValue)
                {
                    GL.Vertex(circleCenter + circleRadius * (v0.Value.x * circleBaseX + v0.Value.y * circleBaseY));
                    GL.Vertex(circleCenter + circleRadius * (v.x * circleBaseX + v.y * circleBaseY));
                }
                v0 = v;
            }

            GL.End();
            GL.PopMatrix();
        }
    }

    public void initCircleLines(Vector3 clickPosition)
    {
        List<Vector2> circleLinesList = new List<Vector2>();
        //Debug.Log(transform.InverseTransformPoint(clickPosition).magnitude);
        Vector3 circleNormal = transform.InverseTransformPoint(clickPosition);
        circleCenter = circleNormal;
        circleBaseX = Vector3.Cross(new Vector3(0f, 1f, 0f), circleNormal).normalized;
        circleBaseY = Vector3.Cross(circleBaseX, circleNormal).normalized;
        for (float theta = 0f; theta < 2f * Mathf.PI + 0.01f; theta += deltaAngle)
        {
            circleLinesList.Add(new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)));
        }
        circleLines = circleLinesList.ToArray();
    }

}