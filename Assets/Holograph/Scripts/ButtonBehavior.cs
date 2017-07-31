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

        public Color HoverHighlight;
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
            _cam = Camera.main.transform;
            _objectMaterial = GetComponent<MeshRenderer>().material;
            _menuBehavior = transform.parent.GetComponent<MenuBehavior>();
        }

        private void Update()
        {
        }

        public void initLayout(MenuBehavior.JNodeMenu.NodeMenuItem nodeMenuItem)
        {
            _buttonName = nodeMenuItem.name;
            MethodName = nodeMenuItem.methodName;
            _color = nodeMenuItem.color;
            _texture = nodeMenuItem.texture;
        }
    }
}