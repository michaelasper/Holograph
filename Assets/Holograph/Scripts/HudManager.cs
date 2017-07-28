using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holograph
{
    public class HudManager : MonoBehaviour
    {
        [Range(.1f, .3f)]
        public float moveLerp = .2f;
        private Transform cam;
        private Vector3 moveTarget;
        public bool isGazedAt;
        void Start()
        {
            cam = Camera.main.transform;
        }

        void Update()
        {
            if (!isGazedAt)
            {
                var headPosition = cam.position;
                var gazeDirection = cam.forward;
                gazeDirection.y = 0f;
                moveTarget = headPosition + new Vector3(0f, -.8f, 0f) + gazeDirection.normalized * 1f;
            }
            transform.position = Vector3.Lerp(transform.position, moveTarget, moveLerp);
        }
        
        public void selectButton(Transform clickedButton)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform childButton = transform.GetChild(i);
                childButton.GetComponent<HudButtonBehavior>().switchSelected(clickedButton == childButton);
            }
        }
    }
}