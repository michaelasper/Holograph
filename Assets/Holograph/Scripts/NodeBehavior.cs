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

    public class NodeBehavior : MonoBehaviour, IInputHandler, IFocusable
    {
        private static Material _lineMaterial;

        public MapManager MapManager;

        public LinkedList<GameObject> Neighborhood = new LinkedList<GameObject>();

        public Dictionary<string, string> NodeInfo { set; get; }

        private bool[] visible;

        public int Index { get; set; }

        public string _id { get; set; }

        private TextMesh textLabel;

        public void OnInputDown(InputEventData eventData)
        {
            //it doesn't work
        }

        public void OnInputUp(InputEventData eventData)
        {
            MapManager.TogglesMenuWithNetworking(Index);
        }

        public void OnRenderObject()
        {
            DrawLines();
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

        private void DrawLines()
        {
            GL.PushMatrix();
            GL.Begin(GL.LINES);
            _lineMaterial.SetPass(0);
            GL.Color(Color.gray);

            foreach (var n in Neighborhood)
            {
                if (visible[Index] && visible[n.GetComponent<NodeBehavior>().Index])
                {
                    var s = n.transform.position;
                    var t = transform.position;
                    var dir = (t - s).normalized;
                    GL.Vertex(s);
                    GL.Vertex(t);
                }

            }

            GL.End();
            GL.PopMatrix();
        }

        private void Start()
        {
            MapManager = transform.parent.GetComponent<MapManager>();
            CreateLineMaterial();
            int currentCase = MapManager.currentCase;
            CaseObject targetCaseObject = MapManager.caseObjects.FirstOrDefault(caseObject => caseObject.CaseId == currentCase);
            visible = targetCaseObject.Visible;
            textLabel = GetComponentInChildren<TextMesh>();
            textLabel.gameObject.SetActive(false);
        }

        public void OnFocusEnter()
        {
            textLabel.text = this.NodeInfo["Name"];
            textLabel.gameObject.SetActive(true);
        }

        public void OnFocusExit()
        {
            textLabel.gameObject.SetActive(false);
        }
    }

}