// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;

    using HoloToolkit.Sharing;
    using HoloToolkit.Sharing.VoiceChat;

    using UnityEngine;

    /// <summary>
    /// This script has functions to control the story
    /// </summary>
    public class StoryManager : MonoBehaviour
    {
        /// <summary>
        /// Default cursor
        /// </summary>
        public AudioSource AudioSource;

        /// <summary>
        /// Audio clip for expanding the graph
        /// </summary>
        public AudioClip ExpandGraphSound;

        /// <summary>
        /// Hash code for fades-in trigger
        /// </summary>
        private int fadesInHash;

        /// <summary>
        /// The globe.
        /// </summary>
        private GameObject globe;

        /// <summary>
        /// The user side panel
        /// </summary>
        //private GameObject UserPanel;

        /// <summary>
        /// The globe animator.
        /// </summary>
        private Animator globeAnimator;

        /// <summary>
        /// The globe behavior.
        /// </summary>
        private GlobeBehavior globeBehavior;

        /// <summary>
        /// The map manager.
        /// </summary>
        private MapManager mapManager;

        /// <summary>
        /// The report panel.
        /// </summary>
        private GameObject reportPanel;

        /// <summary>
        /// The story action.
        /// </summary>
        public enum StoryAction : byte
        {
            /// <summary>
            /// The enter default story.
            /// </summary>
            EnterDefaultStory,

            /// <summary>
            /// The expand.
            /// </summary>
            Expand,

            /// <summary>
            /// The list info.
            /// </summary>
            ListInfo,

            /// <summary>
            /// The reset story.
            /// </summary>
            ResetStory
        }

        public void TriggerStoryWithNetworking(StoryAction action, params int[] args)
        {
            this.triggerStory(action, args);
            NetworkMessages.Instance.SendStoryControl((byte)action, args);
        }

        /// <summary>
        /// The trigger story.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when a wrong number of arguments are passed
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when story action is not supported
        /// </exception>
        private void triggerStory(StoryAction action, params int[] args)
        {
            switch (action)
            {
                case StoryAction.EnterDefaultStory:
                    this.EnterDefaultStory(args[0]);
                    break;
                case StoryAction.ListInfo:
                    if (args == null || args.Length != 1)
                    {
                        throw new ArgumentException("ListInfo expects one parameter");
                    }

                    break;
                case StoryAction.Expand:
                    if (args == null || args.Length != 1)
                    {
                        throw new ArgumentException("Expand expects one parameter");
                    }

                    this.AudioSource.PlayOneShot(ExpandGraphSound);
                    this.Expand(this.mapManager.getCurrentCaseObject().NodeObject[args[0]].transform);
                    break;
                case StoryAction.ResetStory:
                    this.ResetStory();
                    break;
                default:
                    throw new NotSupportedException("Story Action not supported");
            }

        }

        /// <summary>
        /// Handles the story control network messages
        /// </summary>
        /// <param name="message">
        /// The network message.
        /// </param>
        public void HandleStoryControlNetworkMessage(NetworkInMessage message)
        {
            message.ReadInt64();
            var action = (StoryAction)message.ReadByte();
            int l = message.ReadInt32();
            var args = new int[l];
            for (var i = 0; i < l; ++i)
            {
                args[i] = message.ReadInt32();
            }

            this.triggerStory(action, args);
        }

        /// <summary>
        /// The enter default story.
        /// </summary>
        private void EnterDefaultStory(int caseId)
        {
            this.globeBehavior.DefaultStoryEntry(caseId);
        }

        /// <summary>
        /// The expand.
        /// </summary>
        /// <param name="expandedNode">
        /// The expanded node.
        /// </param>
        private void Expand(Transform expandedNode)
        {
            foreach (var node in expandedNode.GetComponent<NodeBehavior>().Neighborhood)
            {
                var currentCaseObject = this.mapManager.getCurrentCaseObject();
                currentCaseObject.Visible[node.GetComponent<NodeBehavior>().Index] = true;
                node.SetActive(true);
            }

            this.mapManager.PositionNodes();
        }

        /// <summary>
        /// Resets the graph and returns back to the globe
        /// </summary>
        private void ResetStory()
        {
            this.globe.gameObject.SetActive(true);
            this.globe.GetComponent<Collider>().enabled = true;
            this.globe.GetComponent<GlobeBehavior>().rotating = true;
            this.reportPanel.SetActive(true);
            if (this.globeAnimator != null && this.globeAnimator.isInitialized)
            {
                this.globeAnimator.SetTrigger(this.fadesInHash);
            }

            this.mapManager.HideMap();
        }

        /// <summary>
        /// Unity calls this
        /// </summary>
        private void Start()
        {
            this.globe = transform.Find("Globe").gameObject;
            this.mapManager = this.GetComponentInChildren<MapManager>();
            this.reportPanel = transform.Find("ReportPanel").gameObject;
            this.globeAnimator = this.globe.GetComponent<Animator>();
            this.fadesInHash = Animator.StringToHash("fadesIn");
            this.globeBehavior = this.globe.GetComponent<GlobeBehavior>();

            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.StoryControl] = this.HandleStoryControlNetworkMessage;
        }

    }

}