using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Sharing;

namespace Holograph
{
    public class StoryManager : MonoBehaviour
    {
        //This script has functions to control the story
        private GameObject globe;
        private MapManager mapManager;
        private GameObject reportPanel;
        private Animator globeAnimator;
        private GlobeBehavior globeBehavior;
        private GameObject infoPanel;
        private InfoPanelBehavior infoPanelBehavior;
        private int fadesInHash;

        public enum StoryAction : byte
        {
            EnterDefaulStory,
            Expand,
            ListInfo,
            ResetStory
        }

        private void ResetStory()
        {
            globe.gameObject.SetActive(true);
            globe.GetComponent<Collider>().enabled = true;
            globe.GetComponent<GlobeBehavior>().rotating = true;
            reportPanel.SetActive(true);
            if (globeAnimator != null && globeAnimator.isInitialized)
            {
                globeAnimator.SetTrigger(fadesInHash);
            }
            mapManager.hideNodes();
        }

        private void Expand(Transform expandedNode)
        {
            foreach (GameObject node in expandedNode.GetComponent<NodeBehavior>().neighborhood)
            {

                mapManager.visible[node.GetComponent<NodeBehavior>().id] = true;
                node.SetActive(true);
            }
            mapManager.positionNodes();
            infoPanelBehavior.ClosePanel();
        }

        private void ListInfo(Transform node)
        {
            infoPanel.SetActive(true);
            NodeInfo nodeInfo = node.GetComponent<NodeBehavior>().nodeInfo;
            infoPanelBehavior.UpdateInfo(nodeInfo);
        }

        private void EnterDefaultStory()
        {
            globeBehavior.DefaultStoryEntry();
        }

        void Start()
        {
            globe = transform.Find("Globe").gameObject;
            mapManager = GetComponentInChildren<MapManager>();
            reportPanel = transform.Find("ReportPanel").gameObject;
            globeAnimator = globe.GetComponent<Animator>();
            fadesInHash = Animator.StringToHash("fadesIn");
            globeBehavior = globe.GetComponent<GlobeBehavior>();
            infoPanel = transform.Find("InfoPanel").gameObject;
            infoPanelBehavior = infoPanel.GetComponent<InfoPanelBehavior>();

            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.StoryControl] = UpdateStoryControl;
        }

        void Update()
        {

        }

        public void TriggerStory(StoryAction action, params int[] args)
        {
            switch (action)
            {
                case StoryAction.EnterDefaulStory:
                    EnterDefaultStory();
                    break;
                case StoryAction.ListInfo:
                    if (args == null || args.Length != 1)
                    {
                        throw new ArgumentException("ListInfo expects one parameter");
                    }
                    ListInfo(mapManager.nodeObject[args[0]].transform);
                    break;
                case StoryAction.Expand:
                    if (args == null || args.Length != 1)
                    {
                        throw new ArgumentException("Expand expects one parameter");
                    }
                    Expand(mapManager.nodeObject[args[0]].transform);
                    break;
                case StoryAction.ResetStory:
                    ResetStory();
                    break;
                default:
                    throw new NotSupportedException("Story Action not supported");
            }
            NetworkMessages.Instance.SendStoryControl((byte)action, args);
        }

        public void UpdateStoryControl(NetworkInMessage msg)
        {
            long userId = msg.ReadInt64();
            StoryAction action = (StoryAction) msg.ReadByte();
            int l = msg.ReadInt32();
            int[] args = new int[l];
            for (int i = 0; i < l; ++i)
            {
                args[i] = msg.ReadInt32();
            }
            TriggerStory(action, args);
        }
    }
}