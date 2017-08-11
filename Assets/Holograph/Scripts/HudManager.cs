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
        public GameObject ASCPanel;
        public GameObject UserListSidePanel;

        private Transform cam;

        private Vector3 moveTarget;

        public void StartStory()
        {
            this.MainStoryManager.TriggerStoryWithNetworking(StoryManager.StoryAction.EnterDefaultStory, 0);
            this.CasePanel.SetActive(false);
            this.ASCPanel.SetActive(false);
        }

        public void clickButtonUp(Transform clickedButton)
        {
            switch (clickedButton.name)
            {
                case "CASES":
                    this.CasePanel.SetActive(!this.CasePanel.activeSelf);
                    this.SelectButton(clickedButton);
                    break;
                case "USERS":
                    this.UserListSidePanel.SetActive(!this.UserListSidePanel.activeSelf);
                    this.SelectButton(clickedButton);
                    break;
                case "ASC":
                    this.ASCPanel.SetActive(!this.ASCPanel.activeSelf);
                    this.SelectButton(clickedButton);
                    break;
                case "HOME":
                    this.MainStoryManager.TriggerStoryWithNetworking(StoryManager.StoryAction.ResetStory);
                    break;
                case "Case Panel":
                    this.MainStoryManager.TriggerStoryWithNetworking(StoryManager.StoryAction.EnterDefaultStory);
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