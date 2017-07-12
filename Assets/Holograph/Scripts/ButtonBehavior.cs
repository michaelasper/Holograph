using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Holograph
{
    public class ButtonBehavior : MonoBehaviour, IInputHandler, IFocusable
    {
        public bool ButtonPressed;
        public Animator IconAnimator;
        // Use this for initialization
        void Start()
        {
            ButtonPressed = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnInputDown(InputEventData eventData)
        {

        }

        public void OnInputUp(InputEventData eventData)
        {
            //    Transform child = transform.GetChild(0);
            //    child.gameObject.SetActive(!child.gameObject.activeSelf);
            //}
            ButtonPressed = !ButtonPressed;
            IconAnimator.SetBool("Button_1", ButtonPressed);
        }
        public void OnFocusEnter()
        {

        }

        public void OnFocusExit()
        {
        }

    }
}

