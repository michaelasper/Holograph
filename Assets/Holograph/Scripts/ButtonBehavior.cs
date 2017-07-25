using HoloToolkit.Sharing;
using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine;

namespace Holograph
{
    public class ButtonBehavior : MonoBehaviour, IInputHandler, IFocusable
    {
        [Range(1f, 2f)]
        public float hightlight = 1.5f;
        private bool isGazedAt;
        private Transform cam;
        private Material objectMaterial;
        //public bool ButtonPressed;
        private string buttonName;
        public string methodName;
        private string color;
        private string texture;
        private MenuBehavior menuBehavior;
        //public Animator IconAnimator;
        // Use this for initialization
        void Start()
        {
            //ButtonPressed = false;
            cam = Camera.main.transform;
            objectMaterial = GetComponent<MeshRenderer>().material;
            menuBehavior = transform.parent.GetComponent<MenuBehavior>();
        }

        void Update()
        {

            RaycastHit hit;
            Ray ray = new Ray(cam.position, cam.forward);
            if (Physics.Raycast(ray, out hit) && hit.transform == this.transform)
            {
                if (!isGazedAt)
                {
                    isGazedAt = true;
                    objectMaterial.color *= hightlight;
                }
            }
            else if (isGazedAt)
            {
                isGazedAt = false;
                objectMaterial.color /= hightlight;
            }

        }

        public void OnInputDown(InputEventData eventData)
        {

        }

        public void OnInputUp(InputEventData eventData)
        {
            //ButtonPressed = !ButtonPressed;
            //IconAnimator.SetBool("Button_1", ButtonPressed);
            //Debug.Log("button " + buttonName + " is clicked");
            menuBehavior.CloseMenu();
            menuBehavior.Invoke(methodName, 0f);
            NetworkMessages.Instance.SendRadialMenuClickIcon(methodName);
            //NetworkMessages.Instance.SendMenuAnimationHash(Animator.StringToHash("Button_1"), NetworkMessages.AnimationTypes.Boolean, ButtonPressed ? 1 : 0); 
        }

        public void OnFocusEnter()
        {

        }

        public void OnFocusExit()
        {

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

