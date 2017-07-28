using HoloToolkit.Sharing;
using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine;

namespace Holograph
{
    public class ButtonBehavior : MonoBehaviour, IInputHandler, IFocusable
    {
        public Color hoverHighlight;
        private Transform cam;
        private Material objectMaterial;
        private string buttonName;
        public string methodName;
        private string color;
        private string texture;
        private MenuBehavior menuBehavior;
        void Start()
        {
            cam = Camera.main.transform;
            objectMaterial = GetComponent<MeshRenderer>().material;
            menuBehavior = transform.parent.GetComponent<MenuBehavior>();
        }

        void Update()
        {

        }

        public void OnInputDown(InputEventData eventData)
        {

        }

        public void OnInputUp(InputEventData eventData)
        {
            OnFocusExit();
            menuBehavior.CloseMenu();
            menuBehavior.Invoke(methodName, 0f);
            NetworkMessages.Instance.SendRadialMenuClickIcon(methodName);
        }

        public void OnFocusEnter()
        {
            objectMaterial.color = hoverHighlight;
        }

        public void OnFocusExit()
        {
            objectMaterial.color = Color.white;
        }

        public void initLayout(MenuBehavior.JNodeMenu.NodeMenuItem nodeMenuItem)
        {
            this.buttonName = nodeMenuItem.name;
            this.methodName = nodeMenuItem.methodName;
            this.color = nodeMenuItem.color;
            this.texture = nodeMenuItem.texture;
        }
    }
}