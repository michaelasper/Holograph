using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holograph
{
    public class NodeBehavior : MonoBehaviour
    {

        public int id { set; get; }

        public bool RandomLines = false;

        public LinkedList<GameObject> neighborhood = new LinkedList<GameObject>();
        //private bool CanRenderLines = false;
        private static Material lineMaterial;
        //public const int SEED = 1234;
        private NodeInfo nodeInfo;

        //private Renderer renderer;
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
        // Use this for initialization
        void Start()
        {
            nodeInfo = new NodeInfo();
            //neighborhood = new LinkedList<GameObject>();
        }

        // Update is called once per frame
        void Update()
        {



        }
        public void ChangeColor(Material color)
        {
            //this.color = color;
            // this.gameObject.mate
            //Debug.Log(renderer);
            GetComponent<Renderer>().material = color;
        }
        public void ChangeName(string text)
        {
            textMesh.text = text;
        }


        public void OnRenderObject()
        {
            //if(CanRenderLines) DrawLine(StartNode.transform.position);
            DrawLines();
        }

        private void DrawLines()
        {
            CreateLineMaterial();
            GL.PushMatrix();
            GL.Begin(GL.LINES);
            lineMaterial.SetPass(0);
            GL.Color(Color.white);
            bool[] visible = this.transform.parent.GetComponent<MapManager>().visible;
            foreach (GameObject n in neighborhood)
            {
                if (visible[id] && visible[n.GetComponent<NodeBehavior>().id])
                {
                    GL.Vertex(n.transform.position);
                    GL.Vertex(this.transform.position);
                }
            }
            GL.End();
            GL.PopMatrix();
        }

        private class NodeInfo
        {
            public Dictionary<string, string> info;

            public string type;

            public NodeInfo()
            {
                this.info = new Dictionary<string, string>();
                this.type = "device";
            }

            public void AddInfo(string category, string information)
            {
                info.Add(category, information);
            }
        }
    }
}