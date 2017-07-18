using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holograph
{
    public class QuadHoverEffect : MonoBehaviour
    {
        private Transform cam;
        private bool isGazedAt;
        private static Material mat;
        private Vector3[] quadCoords;

        [Tooltip("Width and height of the shade rectangle.")]
        public Vector2 size = new Vector2(.2f, .2f);
        

        // Use this for initialization
        void Start()
        {
            cam = Camera.main.transform;
            CreateMaterial();
            quadCoords = new Vector3[4];
            quadCoords[0] = size.x * this.transform.right + size.y * this.transform.up - .05f * this.transform.forward;
            quadCoords[1] = size.x * this.transform.right - size.y * this.transform.up - .05f * this.transform.forward;
            quadCoords[2] = -size.x * this.transform.right - size.y * this.transform.up - .05f * this.transform.forward;
            quadCoords[3] = -size.x * this.transform.right + size.y * this.transform.up - .05f * this.transform.forward;
        }

        // Update is called once per frame
        void Update()
        {
            RaycastHit hit;
            Ray ray = new Ray(cam.position, cam.forward);
            if (Physics.Raycast(ray, out hit) && hit.transform == this.transform)
            {
                isGazedAt = true;
            }
            else
            {
                isGazedAt = false;
            }
        }

        static void CreateMaterial()
        {
            if (!mat)
            {
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                mat = new Material(shader);
                mat.hideFlags = HideFlags.HideAndDontSave;
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                mat.SetInt("_ZWrite", 0);
            }
        }

        public void OnRenderObject()
        {
            if (isGazedAt)
            {
                GL.PushMatrix();
                GL.MultMatrix(transform.localToWorldMatrix);
                mat.SetPass(0);
                GL.Begin(GL.QUADS);
                GL.Color(new Color(.1f, .1f, .1f, 1f));
                for (int i = 0; i < 4; ++i)
                {
                    GL.Vertex(quadCoords[i]);
                }
                GL.End();
                GL.PopMatrix();
            }
        }
    }
}