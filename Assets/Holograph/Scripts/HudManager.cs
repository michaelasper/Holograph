using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace Holograph
{
    public class HudManager : MonoBehaviour, IFocusable
    {
        public StoryManager storyManager;
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
                moveTarget = headPosition + new Vector3(0f, -.5f, 0f) + gazeDirection.normalized * 1f;
            }
            transform.position = Vector3.Lerp(transform.position, moveTarget, moveLerp);
        }
        
        private void selectButton(Transform selectedButton)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform childButton = transform.GetChild(i);
                childButton.GetComponent<HudButtonBehavior>().switchSelected(selectedButton == childButton);
            }

        }

        public void clickButtonUp(Transform clickedButton)
        {
            switch (clickedButton.name)
            {
                case "CASES":
                    storyManager.DefaultStoryEntry();
                    break;
                case "USERS":
                case "STATS":
                    selectButton(clickedButton);
                    break;
                case "HOME":
                    storyManager.ResetStory();
                    break;
                default:
                    break;
            }
        }

        public void OnFocusEnter()
        {
            isGazedAt = true;
        }

        public void OnFocusExit()
        {
            isGazedAt = false;
        }
    }
}