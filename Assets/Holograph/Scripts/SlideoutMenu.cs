using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Holograph
{
    public class SlideoutMenu : MonoBehaviour, IInputHandler
    {
        public float MenuLength = .5f;
        public float SlidingSpeedPercentage;
        public Color MenuColor = new Color(.5f, .5f, .5f);

        private Animator slideoutAnimator;
        private int slidesHash;
        public float currMenuLength;
        private float t;
        private float yTop, yBottom;
        private static Material mat;

        void IInputHandler.OnInputDown(InputEventData eventData)
        {
            //throw new NotImplementedException();
        }

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            slideoutAnimator.SetTrigger(slidesHash);
            t = 0f;
        }

        // Use this for initialization
        void Start()
        {
            slideoutAnimator = GetComponent<Animator>();
            slidesHash = Animator.StringToHash("slides");
            yTop = 1f / 2;
            yBottom = -1f / 2;
            CreateMaterial();
        }

        // Update is called once per frame
        void Update()
        {
            if (t < 1f)
            {
                float speed = MenuLength * SlidingSpeedPercentage;
                currMenuLength = Mathf.Lerp(0f, MenuLength, t);
                t += speed * Time.deltaTime;
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
            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            mat.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.Color(MenuColor);
            GL.Vertex3(.5f, yTop, 0f);
            GL.Vertex3(.5f + currMenuLength, yTop, 0f);
            GL.Vertex3(.5f + currMenuLength, yBottom, 0f);
            GL.Vertex3(.5f, yBottom, 0f);
            GL.End();
            GL.PopMatrix();
        }
    }
}