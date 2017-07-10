using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Holograph
{
    public class CubeClick : MonoBehaviour, IInputHandler
    {
        public GameObject globe;
        private Animator globeAnimator;
        public Animator nodeAnimator;
        private int fadesInHash;
        private int disappearsHash;

        void IInputHandler.OnInputDown(InputEventData eventData)
        {
            //throw new NotImplementedException();
        }

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            globe.SetActive(true);
            globe.GetComponent<Collider>().enabled = true;
            globe.GetComponent<GlobeBehavior>().rotating = true;
            if (globeAnimator != null && globeAnimator.isInitialized)
            {
                globeAnimator.SetTrigger(fadesInHash);
                NetworkMessages.Instance.SendAnimationHash(fadesInHash, NetworkMessages.AnimationTypes.Trigger);
            }
            if (nodeAnimator != null && nodeAnimator.isInitialized)
            {
                nodeAnimator.SetTrigger(disappearsHash);
                NetworkMessages.Instance.SendAnimationHash(disappearsHash, NetworkMessages.AnimationTypes.Trigger);
            }
        }

        // Use this for initialization
        void Start()
        {
            fadesInHash = Animator.StringToHash("fadesIn");
            disappearsHash = Animator.StringToHash("disappears");
            globeAnimator = globe.GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}