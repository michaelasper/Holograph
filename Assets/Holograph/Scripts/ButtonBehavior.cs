using HoloToolkit.Sharing;
using HoloToolkit.Unity.InputModule;
using System;
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

        void Update()
        {

        }

        public void OnInputDown(InputEventData eventData)
        {

        }

        public void OnInputUp(InputEventData eventData)
        {
            ButtonPressed = !ButtonPressed;
            IconAnimator.SetBool("Button_1", ButtonPressed);

            NetworkMessages.Instance.SendMenuAnimationHash(Animator.StringToHash("Button_1"), NetworkMessages.AnimationTypes.Boolean, ButtonPressed ? 1 : 0); 
        }

        public void OnFocusEnter()
        {

        }

        public void OnFocusExit()
        {

        }
    }
}

