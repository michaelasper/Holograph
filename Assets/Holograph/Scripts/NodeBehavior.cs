// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using HoloToolkit.Unity.InputModule;

    using UnityEngine;
    using UnityEngine.Rendering;

    public class NodeBehavior : MonoBehaviour, IInputHandler
    {
        private static Material _lineMaterial;

        public MapManager MapManager;

        public LinkedList<GameObject> Neighborhood = new LinkedList<GameObject>();

        public NodeInfo NodeInfo;

        private float _nodeRadius;

        // public TextMesh TextMesh;
        public int Index { get; set; }

        public string _id { get; set; }

        public void OnInputDown(InputEventData eventData)
        {
            // throw new NotImplementedException();
        }

        public void OnInputUp(InputEventData eventData)
        {
            MapManager.TogglesMenu(Index);
        }

        public void OnRenderObject()
        {
            DrawLines();
        }

        public void SetNodeInfo(NodeInfo info)
        {
            NodeInfo = info;
            ChangeName(info.GetProperty("name"));
        }

        private static void CreateLineMaterial()
        {
            if (_lineMaterial)
            {
                return;
            }

            var shader = Shader.Find("Hidden/Internal-Colored");
            _lineMaterial = new Material(shader)
                                {
                                    hideFlags = HideFlags.HideAndDontSave
                                };
            _lineMaterial.SetInt("_SrcBlend", (int)BlendMode.One);
            _lineMaterial.SetInt("_DstBlend", (int)BlendMode.One);
            _lineMaterial.SetInt("_Cull", (int)CullMode.Off);
            _lineMaterial.SetInt("_ZWrite", 0);
        }

        private void ChangeName(string text)
        {
            // TextMesh.text = text;
        }

        private void DrawLines()
        {
            CreateLineMaterial();
            GL.PushMatrix();
            GL.Begin(GL.LINES);
            _lineMaterial.SetPass(0);
            GL.Color(Color.gray);

            int currentCase = MapManager.currentCase;
            var targetCaseObject = MapManager.caseObjects.FirstOrDefault(caseObject => caseObject.CaseId == currentCase);
            var visible = targetCaseObject.Visible;
            foreach (var n in Neighborhood)
            {
                if (visible[Index] && visible[n.GetComponent<NodeBehavior>().Index])
                {
                    var s = n.transform.position;
                    var t = transform.position;
                    var dir = (t - s).normalized;
                    s += dir * _nodeRadius;
                    t -= dir * _nodeRadius;
                    GL.Vertex(s);
                    GL.Vertex(t);
                }
            }

            GL.End();
            GL.PopMatrix();
        }

        // Use this for initialization
        private void Start()
        {
            MapManager = transform.parent.GetComponent<MapManager>();
            _nodeRadius = 0f; // .0005f / transform.localScale.x;
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}