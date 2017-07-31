using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Rendering;

namespace Holograph
{
    public class SlideoutMenu : MonoBehaviour, IInputHandler
    {
        private static Material mat;
        public float CurrMenuLength;
        public Color MenuColor = new Color(.5f, .5f, .5f);
        public float MenuLength = .5f;

        private Animator _slideoutAnimator;
        private int _slidesHash;
        public float SlidingSpeedPercentage;
        private float _t;
        private float _yTop, _yBottom;

        void IInputHandler.OnInputDown(InputEventData eventData)
        {
            //throw new NotImplementedException();
        }

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            _slideoutAnimator.SetTrigger(_slidesHash);
            _t = 0f;
        }

        // Use this for initialization
        private void Start()
        {
            _slideoutAnimator = GetComponent<Animator>();
            _slidesHash = Animator.StringToHash("slides");
            _yTop = 1f / 2;
            _yBottom = -1f / 2;
            CreateMaterial();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!(_t < 1f)) return;
            var speed = MenuLength * SlidingSpeedPercentage;
            CurrMenuLength = Mathf.Lerp(0f, MenuLength, _t);
            _t += speed * Time.deltaTime;
        }

        private static void CreateMaterial()
        {
            if (!mat)
            {
                var shader = Shader.Find("Hidden/Internal-Colored");
                mat = new Material(shader) {hideFlags = HideFlags.HideAndDontSave};
                mat.SetInt("_SrcBlend", (int) BlendMode.One);
                mat.SetInt("_DstBlend", (int) BlendMode.One);
                mat.SetInt("_Cull", (int) CullMode.Off);
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
            GL.Vertex3(.5f, _yTop, 0f);
            GL.Vertex3(.5f + CurrMenuLength, _yTop, 0f);
            GL.Vertex3(.5f + CurrMenuLength, _yBottom, 0f);
            GL.Vertex3(.5f, _yBottom, 0f);
            GL.End();
            GL.PopMatrix();
        }
    }
}