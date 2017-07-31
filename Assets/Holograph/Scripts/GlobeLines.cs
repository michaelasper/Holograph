// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

using UnityEngine;
using UnityEngine.Rendering;

public class GlobeLines : MonoBehaviour
{
    private static Material lineMaterial;

    public float fadeAmt;

    public float moveRange;

    public int pointCount = 30;

    // When added to an object, draws colored rays from the
    // transform position.
    // public int lineCount = 5;
    public float radius;

    public float swapsPerSec;

    private float[] moveAmt;

    private float[] moveSpeed;

    private float[] offset;

    private Vector3[] points;

    private int[] reordered;

    private float swapsToDo;

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        swapsToDo += Time.deltaTime * swapsPerSec;
        while (swapsToDo > 1f)
        {
            swapsToDo -= 1f;
            SwapPoints();
        }

        CreateLineMaterial();
        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        for (var i = 0; i < pointCount; ++i)
        {
            float sin = Mathf.Sin(Time.time * moveSpeed[i] + offset[i]);
            float newRadius = .6f + sin * moveRange * moveAmt[i];
            points[i] = points[i].normalized * newRadius;
            if (i < pointCount - 1)
            {
                float a = sin / 2f + .5f;
                float aOverFour = a / 4f;
                GL.Color(new Color(.05f + aOverFour, .2f + aOverFour, .3f + aOverFour, 1f) * fadeAmt);
                GL.Vertex(points[i]);
                GL.Vertex(points[i + 1]);

                GL.Vertex(points[reordered[i]]);
                GL.Vertex(points[reordered[i + 1]]);
            }
        }

        GL.End();
        GL.PopMatrix();
    }

    private static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            var shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;

            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)BlendMode.One);
            lineMaterial.SetInt("_DstBlend", (int)BlendMode.One);

            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)CullMode.Off);

            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    private void Start()
    {
        points = new Vector3[pointCount];
        reordered = new int[pointCount];
        moveAmt = new float[pointCount];
        offset = new float[pointCount];
        moveSpeed = new float[pointCount];
        for (var p = 0; p < pointCount; p++)
        {
            points[p] = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            moveAmt[p] = Random.Range(0, .5f);
            offset[p] = Random.Range(0f, Mathf.PI * 2f);
            moveSpeed[p] = Random.Range(.5f, 2f);
            reordered[p] = p;
        }

        for (var p = 0; p < pointCount; ++p)
        {
            int temp = reordered[p];
            int i = Random.Range(0, pointCount - 1);
            reordered[p] = reordered[i];
            reordered[i] = temp;
        }
    }

    private void SwapPoints()
    {
        int i = Random.Range(0, pointCount);
        int j = Random.Range(0, pointCount);
        var temp = points[i];
        points[i] = points[j];
        points[j] = temp;

        int r = Random.Range(0, pointCount);
        points[r] = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        points[r] = points[r].normalized * .6f;
    }
}