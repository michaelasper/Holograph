using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Rendering;

namespace Holograph
{
    public class NodeBehavior : MonoBehaviour, IInputHandler
    {
        private static Material _lineMaterial;

        public MapManager MapManager;
        public LinkedList<GameObject> Neighborhood = new LinkedList<GameObject>();

        public NodeInfo NodeInfo;

        private float _nodeRadius;
        //public TextMesh TextMesh;
        public int id { set; get; }

        public void OnInputUp(InputEventData eventData)
        {
            MapManager.TogglesMenu(id);
        }

        public void OnInputDown(InputEventData eventData)
        {
            //throw new NotImplementedException();
        }

        private static void CreateLineMaterial()
        {
            if (_lineMaterial) return;
            var shader = Shader.Find("Hidden/Internal-Colored");
            _lineMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            _lineMaterial.SetInt("_SrcBlend", (int) BlendMode.One);
            _lineMaterial.SetInt("_DstBlend", (int) BlendMode.One);
            _lineMaterial.SetInt("_Cull", (int) CullMode.Off);
            _lineMaterial.SetInt("_ZWrite", 0);
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

        public void SetNodeInfo(NodeInfo info)
        {
            NodeInfo = info;
            ChangeName(info.GetProperty("name"));
        }


        private void ChangeName(string text)
        {
           // TextMesh.text = text;
        }


        public void OnRenderObject()
        {
            DrawLines();
        }

        private void DrawLines()
        {
            CreateLineMaterial();
            GL.PushMatrix();
            GL.Begin(GL.LINES);
            _lineMaterial.SetPass(0);
            GL.Color(Color.gray);
            var visible = transform.parent.GetComponent<MapManager>().Visible;
            foreach (var n in Neighborhood)
                if (visible[id] && visible[n.GetComponent<NodeBehavior>().id])
                {
                    var s = n.transform.position;
                    var t = transform.position;
                    var dir = (t - s).normalized;
                    s += dir * _nodeRadius;
                    t -= dir * _nodeRadius;
                    GL.Vertex(s);
                    GL.Vertex(t);
                }
            GL.End();
            GL.PopMatrix();
        }
    }
}