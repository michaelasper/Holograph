using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Holograph
{
    public class ButtonBehavior : MonoBehaviour, IInputHandler, IFocusable
    {
        private string _buttonName;
        private Transform _cam;
        private string _color;
        private string _texture;

        private MenuBehavior _menuBehavior;
        private Material _objectMaterial;

        private Color HoverHighlight;
        public string MethodName;

        public void OnFocusEnter()
        {
            _objectMaterial.color = HoverHighlight;
        }

        public void OnFocusExit()
        {
            _objectMaterial.color = Color.white;
        }

        public void OnInputDown(InputEventData eventData)
        {
        }

        public void OnInputUp(InputEventData eventData)
        {
            OnFocusExit();
            _menuBehavior.Invoke(MethodName, 0f);
            NetworkMessages.Instance.SendRadialMenuClickIcon(MethodName);
        }

        private void Start()
        {
            ColorUtility.TryParseHtmlString("#BABABAAE", out HoverHighlight);
            _cam = Camera.main.transform;
            _objectMaterial = GetComponent<MeshRenderer>().material;
            _menuBehavior = transform.parent.GetComponent<MenuBehavior>();
        }

        private void Update()
        {
        }

        public void initLayout(MenuBehavior.JNodeMenu.NodeMenuItem nodeMenuItem)
        {
            _buttonName = nodeMenuItem.Name;
            MethodName = nodeMenuItem.MethodName;
        }
    }
}