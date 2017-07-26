using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace Holograph
{
    public class NodeBehavior : MonoBehaviour, IInputHandler
    {

        public MapManager mapManager;
        public int id { set; get; }

        public NodeInfo nodeInfo;
        public TextMesh textMesh;
        public LinkedList<GameObject> neighborhood = new LinkedList<GameObject>();
        private static Material lineMaterial;

        private float nodeRadius;

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

        // Use this for initialization
        void Start()
        {
            mapManager = transform.parent.GetComponent<MapManager>();
            nodeRadius = 0f;// .0005f / transform.localScale.x;
        }

        // Update is called once per frame
        void Update()
        {


        }

        public void SetNodeInfo(NodeInfo info)
        {
            this.nodeInfo = info;
            ChangeName(info.GetProperty("name"));

        }

        //public void ChangeColor(Material color)
        //{
        //    GetComponent<Renderer>().material = color;
        //}


        private void ChangeName(string text)
        {
            textMesh.text = text;
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
            lineMaterial.SetPass(0);
            GL.Color(Color.gray);
            bool[] visible = this.transform.parent.GetComponent<MapManager>().visible;
            foreach (GameObject n in neighborhood)
            {
                if (visible[id] && visible[n.GetComponent<NodeBehavior>().id])
                {
                    Vector3 s = n.transform.position;
                    Vector3 t = this.transform.position;
                    Vector3 dir = (t - s).normalized;
                    s += dir * nodeRadius;
                    t -= dir * nodeRadius;
                    GL.Vertex(s);
                    GL.Vertex(t);
                }
            }
            GL.End();
            GL.PopMatrix();
        }

        public void OnInputUp(InputEventData eventData)
        {
            mapManager.menuClickedOn(id);
        }

        public void OnInputDown(InputEventData eventData)
        {
            //throw new NotImplementedException();
        }
    }
}
