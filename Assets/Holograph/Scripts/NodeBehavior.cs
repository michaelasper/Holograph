﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeBehavior : MonoBehaviour {

    public bool RandomLines = false;

    private GameObject StartNode;
    private bool CanRenderLines = false;
    private static Material lineMaterial;
    public const int SEED = 1234;
    public NodeInfo nodeInfo;

    private Renderer renderer;
    private Material color;
    public TextMesh textMesh;


    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    //Deprecated
    public void EnableLines(GameObject StartNode, int index, int SlideLocation)
    {
        this.StartNode = StartNode;
        if (RandomLines) {
            // We seed random to make a uniform map throughout the hololens and to avoid networking 
            Random.InitState(SEED * index * SlideLocation);
            this.transform.position = (Random.insideUnitSphere * 0.3f) + StartNode.transform.position;
        }
        CanRenderLines = true;
    }
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

       
	}

    public void SetNodeInfo(NodeInfo info)
    {
        this.nodeInfo = info;
        ChangeName(info.GetProperty("name"));

    }

    public void ChangeColor(Material color)
    {
        GetComponent<Renderer>().material = color;
    }


    private void ChangeName(string text)
    {
        textMesh.text = text;
    }


    public void OnRenderObject()
    {
        if(CanRenderLines) DrawLine(StartNode.transform.position);
    }

    private void DrawLine(Vector3 start)
    {
        CreateLineMaterial();
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        lineMaterial.SetPass(0);
        GL.Color(Color.white);
        GL.Vertex(start);
        GL.Vertex(this.transform.position);
        GL.End();
        GL.PopMatrix();
    }

   
}
