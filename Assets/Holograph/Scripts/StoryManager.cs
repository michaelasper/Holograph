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
        private GameObject _globe;
        private MapManager _mapManager;
        private GameObject _reportPanel;
        private Animator _globeAnimator;
        private GlobeBehavior _globeBehavior;
        private GameObject _infoPanel;
        private InfoPanelBehavior _infoPanelBehavior;
        private int _fadesInHash;

        public enum StoryAction : byte
        {
            EnterDefaulStory,
            Expand,
            ListInfo,
            ResetStory
        }

        private void ResetStory()
        {
            _globe.gameObject.SetActive(true);
            _globe.GetComponent<Collider>().enabled = true;
            _globe.GetComponent<GlobeBehavior>().rotating = true;
            _reportPanel.SetActive(true);
            if (_globeAnimator != null && _globeAnimator.isInitialized)
            {
                _globeAnimator.SetTrigger(_fadesInHash);
            }
            _mapManager.HideNodes();
        }

        private void Expand(Transform expandedNode)
        {
            foreach (GameObject node in expandedNode.GetComponent<NodeBehavior>().Neighborhood)
            {

                _mapManager.Visible[node.GetComponent<NodeBehavior>().id] = true;
                node.SetActive(true);
            }
            _mapManager.PositionNodes();
            //_infoPanelBehavior.ClosePanel();
        }

        private void ListInfo(Transform node)
        {
            _infoPanel.SetActive(true);
            NodeInfo nodeInfo = node.GetComponent<NodeBehavior>().NodeInfo;
            _infoPanelBehavior.UpdateInfo(nodeInfo);
        }

        private void EnterDefaultStory()
        {
            _globeBehavior.DefaultStoryEntry();
        }

        void Start()
        {
            _globe = transform.Find("Globe").gameObject;
            _mapManager = GetComponentInChildren<MapManager>();
            _reportPanel = transform.Find("ReportPanel").gameObject;
            _globeAnimator = _globe.GetComponent<Animator>();
            _fadesInHash = Animator.StringToHash("fadesIn");
            _globeBehavior = _globe.GetComponent<GlobeBehavior>();
            
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
                    ListInfo(_mapManager.NodeObject[args[0]].transform);
                    break;
                case StoryAction.Expand:
                    if (args == null || args.Length != 1)
                    {
                        throw new ArgumentException("Expand expects one parameter");
                    }
                    Expand(_mapManager.NodeObject[args[0]].transform);

                    break;
                default:
                    throw new NotSupportedException("Story Action not supported");
            }
            NetworkMessages.Instance.SendStoryControl((byte)action, args);
        }

        public void UpdateStoryControl(NetworkInMessage msg)
        {
            msg.ReadInt64();
            
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