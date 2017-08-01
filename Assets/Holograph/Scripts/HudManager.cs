// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;

    using HoloToolkit.Unity.InputModule;

    using UnityEngine;

    public class HudManager : MonoBehaviour, IFocusable
    {
        public bool isGazedAt;

        [Range(.1f, .3f)]
        public float moveLerp = .2f;

        public StoryManager storyManager;

        private Transform cam;

        private Vector3 moveTarget;

        public void clickButtonUp(Transform clickedButton)
        {
            switch (clickedButton.name)
            {
                case "CASES":
                    storyManager.TriggerStory(StoryManager.StoryAction.EnterDefaultStory);
                    break;
                case "USERS":
                    storyManager.TriggerStory(StoryManager.StoryAction.TogglePanel, 0);
                    selectButton(clickedButton);
                    break;
                case "STATS":
                    storyManager.TriggerStory(StoryManager.StoryAction.TogglePanel, 1);
                    selectButton(clickedButton);
                    break;
                case "HOME":
                    storyManager.TriggerStory(StoryManager.StoryAction.ResetStory);
                    break;
                default: break;
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

        private void selectButton(Transform selectedButton)
        {
            for (var i = 0; i < transform.childCount; ++i)
            {
                var childButton = transform.GetChild(i);
                childButton.GetComponent<HudButtonBehavior>().switchSelected(selectedButton == childButton);
            }
        }

        private void Start()
        {
            cam = Camera.main.transform;
        }

        private void Update()
        {
            if (!isGazedAt)
            {
                var headPosition = cam.position;
                var gazeDirection = cam.forward;
                gazeDirection.y = 0f;
                moveTarget = headPosition + new Vector3(0f, -.7f, 0f) + gazeDirection.normalized * 1f;
            }

            transform.position = Vector3.Lerp(transform.position, moveTarget, moveLerp);
        }
    }
}