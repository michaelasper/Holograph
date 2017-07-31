// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

using System.Collections.Generic;

using UnityEngine;

public class GlobeFrame : MonoBehaviour
{
    public float fadeAmt;

    public Color gridColor;

    public Vector3[] gridPoints;

    public int latitudeCirclesNum;

    // public GameObject globe;
    public Material lineMaterial;

    public int longitudeCirclesNum;

    public int numLinesInCircle = 54;

    private float deltaPhi;

    private float deltaTheta;

    private readonly float rho = .5f;

    // void CreateLineMaterial()
    // {
    // if (!lineMaterial)
    // {
    // //Shader shader = Shader.Find("GlobeFrame-Lines");
    // //lineMaterial = new Material(shader);
    // //lineMaterial.hideFlags = HideFlags.HideAndDontSave;
    // //lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
    // //lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
    // //lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
    // //lineMaterial.SetInt("_ZWrite", 0);
    // }
    // }
    public void OnRenderObject()
    {
        // Debug.Log(gridPoints.Length);
        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(gridColor * fadeAmt);
        for (var i = 0; i < gridPoints.Length; ++i)
        {
            GL.Vertex(gridPoints[i]);
            GL.Vertex(gridPoints[++i]);
        }

        GL.End();
        GL.PopMatrix();
    }

    private void initGrid()
    {
        var gridPointsList = new List<Vector3>();
        float deltaAngle = Mathf.PI * 2f / numLinesInCircle;
        for (float theta = deltaTheta; theta < Mathf.PI; theta += deltaTheta)
        {
            Vector3? pos0 = null;
            float sinTheta = Mathf.Sin(theta);
            float cosTheta = Mathf.Cos(theta);
            float r = rho * sinTheta;
            float z = rho * cosTheta;
            for (float phi = 0; phi < 2 * Mathf.PI + 0.01f; phi += deltaAngle)
            {
                float x = r * Mathf.Cos(phi);
                float y = r * Mathf.Sin(phi);

                var pos1 = new Vector3(x, y, z);
                if (pos0.HasValue)
                {
                    gridPointsList.Add(pos0.Value);
                    gridPointsList.Add(pos1);
                }

                pos0 = pos1;
            }
        }

        for (float phi = 0; phi < Mathf.PI; phi += deltaPhi)
        {
            Vector3? pos0 = null;
            for (float theta = 0; theta < 2 * Mathf.PI + .01f; theta += deltaAngle)
            {
                float r = rho * Mathf.Sin(theta);
                float x = r * Mathf.Cos(phi);
                float y = r * Mathf.Sin(phi);
                float z = rho * Mathf.Cos(theta);

                var pos1 = new Vector3(x, y, z);
                if (pos0.HasValue)
                {
                    gridPointsList.Add(pos0.Value);
                    gridPointsList.Add(pos1);
                }

                pos0 = pos1;
            }
        }

        gridPoints = gridPointsList.ToArray();
    }

    // Use this for initialization
    private void Start()
    {
        deltaTheta = Mathf.PI / (latitudeCirclesNum + .1f);
        deltaPhi = Mathf.PI / (longitudeCirclesNum + .1f);
        initGrid();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}