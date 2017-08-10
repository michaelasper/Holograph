// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

public class GlobeCircleWave : MonoBehaviour
{
    private static Material lineMaterial;

    public float circleRadius;

    public bool showCircle;

    private Vector3 circleBaseX;

    private Vector3 circleBaseY;

    private Vector3 circleCenter;

    private Vector2[] circleLines;

    private readonly float deltaAngle = Mathf.PI / 10f;

    public void initCircleLines(Vector3 clickPosition)
    {
        var circleLinesList = new List<Vector2>();
        var circleNormal = transform.InverseTransformPoint(clickPosition);
        circleCenter = circleNormal;
        circleBaseX = Vector3.Cross(new Vector3(0f, 1f, 0f), circleNormal).normalized;
        circleBaseY = Vector3.Cross(circleBaseX, circleNormal).normalized;
        for (var theta = 0f; theta < 2f * Mathf.PI + 0.01f; theta += deltaAngle)
        {
            circleLinesList.Add(new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)));
        }

        circleLines = circleLinesList.ToArray();
    }

    public void OnRenderObject()
    {
        if (circleCenter != Vector3.zero)
        {
            CreateLineMaterial();
            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            lineMaterial.SetPass(0);
            GL.Begin(GL.LINES);
            float a = 1f - circleRadius / 0.07f;
            GL.Color(new Color(a, a, a, 1f));
            Vector3? v0 = null;
            foreach (var v in circleLines)
            {
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

    private static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            var shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.SetInt("_SrcBlend", (int)BlendMode.One);
            lineMaterial.SetInt("_DstBlend", (int)BlendMode.One);
            lineMaterial.SetInt("_Cull", (int)CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    private void Start()
    {
        showCircle = false;
    }

}