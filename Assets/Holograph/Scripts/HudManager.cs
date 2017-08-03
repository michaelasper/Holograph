// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;

    using HoloToolkit.Unity.InputModule;

    using NUnit.Framework.Constraints;

    using UnityEngine;

    public class HudManager : MonoBehaviour, IFocusable
    {
        public bool IsGazedAt;

        [Range(.1f, .3f)]
        public float MoveLerp = .2f;

        public StoryManager MainStoryManager;

        public GameObject CasePanel;

        private Transform cam;

        private Vector3 moveTarget;

        public void StartStory()
        {
            this.MainStoryManager.TriggerStory(StoryManager.StoryAction.EnterDefaultStory);
        }

        public void clickButtonUp(Transform clickedButton)
        {
            switch (clickedButton.name)
            {
                case "CASES":
                    this.CasePanel.SetActive(!this.CasePanel.activeSelf);
                    ////storyManager.TriggerStory(StoryManager.StoryAction.EnterDefaultStory);
                    break;
                case "USERS":
                    this.MainStoryManager.TriggerStory(StoryManager.StoryAction.TogglePanel, 0);
                    this.SelectButton(clickedButton);
                    break;
                case "STATS":
                    this.MainStoryManager.TriggerStory(StoryManager.StoryAction.TogglePanel, 1);
                    this.SelectButton(clickedButton);
                    break;
                case "HOME":
                    this.MainStoryManager.TriggerStory(StoryManager.StoryAction.ResetStory);
                    break;
                case "Case Panel":
                    this.MainStoryManager.TriggerStory(StoryManager.StoryAction.EnterDefaultStory);
                    break;
                default: break;
            }
        }

        public void OnFocusEnter()
        {
            this.IsGazedAt = true;
        }

        public void OnFocusExit()
        {
            this.IsGazedAt = false;
        }

        private void SelectButton(Transform selectedButton)
        {
            //for (var i = 0; i < transform.childCount; ++i)
            //{
            //    var childButton = transform.GetChild(i);
            //    childButton.GetComponent<HudButtonBehavior>().switchSelected(selectedButton == childButton);
            //}
            selectedButton.GetComponent<HudButtonBehavior>().switchSelected(true);
        }

        private void Start()
        {
            this.cam = Camera.main.transform;
        }

        private void Update()
        {
            if (!IsGazedAt)
            {
                var headPosition = this.cam.position;
                var gazeDirection = this.cam.forward;
                gazeDirection.y = 0f;
                this.moveTarget = headPosition + new Vector3(0f, -.7f, 0f) + (gazeDirection.normalized * 1f);
            }

            transform.position = Vector3.Lerp(transform.position, this.moveTarget, this.MoveLerp);
        }
    }
}