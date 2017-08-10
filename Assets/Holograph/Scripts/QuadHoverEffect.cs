// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;

    using UnityEngine;
    using UnityEngine.Rendering;

    [Obsolete]
    public class QuadHoverEffect : MonoBehaviour
    {
        private static Material mat;
        private Transform cam;
        private bool isGazedAt;
        private Vector3[] quadCoords;

        [Tooltip("Width and height of the shade rectangle.")]
        public Vector2 size = new Vector2(.55f, .55f);

        [Tooltip("Greater value means more transparent.")]
        public float transparency = .1f;

        private void Start()
        {
            cam = Camera.main.transform;
            CreateMaterial();
            quadCoords = new Vector3[4];
            quadCoords[0] = size.x * Vector3.right + size.y * Vector3.up - .05f * Vector3.forward;
            quadCoords[1] = size.x * Vector3.right - size.y * Vector3.up - .05f * Vector3.forward;
            quadCoords[2] = -size.x * Vector3.right - size.y * Vector3.up - .05f * Vector3.forward;
            quadCoords[3] = -size.x * Vector3.right + size.y * Vector3.up - .05f * Vector3.forward;
        }

        private void Update()
        {
            RaycastHit hit;
            var ray = new Ray(cam.position, cam.forward);
            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                isGazedAt = true;
            }

            else
            {
                isGazedAt = false;
            }

        }

        private static void CreateMaterial()
        {
            if (!mat)
            {
                var shader = Shader.Find("Hidden/Internal-Colored");
                mat = new Material(shader);
                mat.hideFlags = HideFlags.HideAndDontSave;
                mat.SetInt("_SrcBlend", (int)BlendMode.One);
                mat.SetInt("_DstBlend", (int)BlendMode.One);
                mat.SetInt("_Cull", (int)CullMode.Off);
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
                GL.Color(new Color(1f, 1f, 1f, 1f) * transparency);
                for (var i = 0; i < 4; ++i)
                {
                    GL.Vertex(quadCoords[i]);
                }

                GL.End();
                GL.PopMatrix();
            }

        }

    }

}