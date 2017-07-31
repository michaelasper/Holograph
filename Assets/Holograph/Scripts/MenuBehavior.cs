// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;
    using System.IO;

    using HoloToolkit.Sharing;

    using UnityEngine;

    /// <summary>
    ///     The menu behavior.
    /// </summary>
    public class MenuBehavior : MonoBehaviour
    {
        /// <summary>
        ///     The enrich panel.
        /// </summary>
        public GameObject EnrichPanel;

        /// <summary>
        ///     The globe.
        /// </summary>
        public GameObject Globe;

        /// <summary>
        ///     The info panel.
        /// </summary>
        public GameObject InfoPanel;

        /// <summary>
        ///     The JSON file asset.
        /// </summary>
        public TextAsset JsonFileAsset;

        /// <summary>
        ///     The mitigate panel.
        /// </summary>
        public GameObject MitigatePanel;

        /// <summary>
        ///     The report panel.
        /// </summary>
        public GameObject ReportPanel;

        /// <summary>
        ///     The stage story manager.
        /// </summary>
        public StoryManager StageStoryManager;

        /// <summary>
        ///     Goes back -- closes panel
        /// </summary>
        public void Back()
        {
            Debug.Log("Back");
        }

        /// <summary>
        ///     Closes all open Panels
        /// </summary>
        public void CloseAllPanels()
        {
            this.InfoPanel.SetActive(false);
            this.EnrichPanel.SetActive(false);
            this.MitigatePanel.SetActive(false);
        }

        /// <summary>
        ///     Closes the hexagonal menu
        /// </summary>
        public void CloseMenu()
        {
            gameObject.SetActive(false);

            NetworkMessages.Instance.SendRadialMenu(-1, false);
        }

        /// <summary>
        ///     Opens up the Enrich Panel on the hexagonal Menu
        /// </summary>
        public void Enrich()
        {
            this.EnrichPanel.SetActive(!this.EnrichPanel.activeSelf);
            this.StageStoryManager.TriggerStory(StoryManager.StoryAction.ListInfo, GetComponentInParent<NodeBehavior>().id);
        }

        /// <summary>
        ///     Expands the graph when icon is hit
        /// </summary>
        public void Expand()
        {
            this.CloseAllPanels();
            this.CloseMenu();
            this.StageStoryManager.TriggerStory(StoryManager.StoryAction.Expand, GetComponentInParent<NodeBehavior>().id);
        }

        /// <summary>
        ///     Handles the network message from a button click
        /// </summary>
        /// <param name="message">
        ///     The network message.
        /// </param>
        public void HandleMenuButtonClickNetworkMessage(NetworkInMessage message)
        {
            message.ReadInt64();
            int l = message.ReadInt32();
            var methodNameChars = new char[l];
            for (var i = 0; i < l; ++i)
            {
                methodNameChars[i] = Convert.ToChar(message.ReadByte());
            }

            var methodName = new string(methodNameChars);
            this.CloseMenu();
            this.Invoke(methodName, 0);
        }

        /// <summary>
        ///     Opens up the Info Panel on the hexagonal Menu
        /// </summary>
        public void ListInfo()
        {
            this.InfoPanel.SetActive(!this.InfoPanel.activeSelf);
        }

        /// <summary>
        ///     Opens up the Mitigate Panel on the hexagonal Menu
        /// </summary>
        public void Mitigate()
        {
            this.MitigatePanel.SetActive(!this.MitigatePanel.activeSelf);
        }

        /// <summary>
        ///     Resets the application back to the globe stage
        /// </summary>
        public void ResetStory()
        {
            this.StageStoryManager.TriggerStory(StoryManager.StoryAction.ResetStory);
        }

        /// <summary>
        ///     Called when instantiated but not active
        /// </summary>
        /// <exception cref="FileNotFoundException">
        ///     JSON file was not found
        /// </exception>
        private void Awake()
        {
            if (this.JsonFileAsset == null)
            {
                throw new FileNotFoundException();
            }

            string json = this.JsonFileAsset.text;
            var nodeMenuItems = JsonUtility.FromJson<JNodeMenu>(json).NodeMenuItems;
            Debug.Log("there are " + nodeMenuItems.Length + " menu items");
            for (var i = 0; i < nodeMenuItems.Length; ++i)
            {
                transform.GetChild(i).GetComponent<ButtonBehavior>().initLayout(nodeMenuItems[i]);
            }

            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.RadialMenuClickIcon] = this.HandleMenuButtonClickNetworkMessage;
        }

        /// <summary>
        ///     The deserialized struct for the JSON File
        /// </summary>
        [Serializable]
        public struct JNodeMenu
        {
            /// <summary>
            ///     The node menu items.
            /// </summary>
            public NodeMenuItem[] NodeMenuItems;

            /// <summary>
            ///     The node menu item.
            /// </summary>
            [Serializable]
            public struct NodeMenuItem
            {
                /// <summary>
                ///     Menu button name
                /// </summary>
                public string Name;

                /// <summary>
                ///     Menu button method
                /// </summary>
                public string MethodName;
            }
        }
    }
}