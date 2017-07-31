using System;
using HoloToolkit.Sharing;
using UnityEngine;

namespace Holograph
{
    public class MenuBehavior : MonoBehaviour
    {
        public GameObject EnrichPanel;
        public GameObject Globe;
        public GameObject InfoPanel;
        public TextAsset JsonFileAsset;

        public GameObject MitigatePanel;
        public GameObject ReportPanel;

        public StoryManager StoryManager;


        private void Awake()
        {
            
            if (JsonFileAsset == null) throw new Exception("JSON File not found");

            var json = JsonFileAsset.text;
            var nodeMenuItems = JsonUtility.FromJson<JNodeMenu>(json).nodeMenuItems;
            Debug.Log("there are " + nodeMenuItems.Length + " menu items");
            for (var i = 0; i < nodeMenuItems.Length; i++)
                transform.GetChild(i).GetComponent<ButtonBehavior>().initLayout(nodeMenuItems[i]);
           
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.RadialMenuClickIcon] =
                UpdateRadialMenuClickIcon;
        }

        /// <summary>
        ///     Expands the graph when icon is hit
        /// </summary>
        public void Expand()
        {
            CloseAllPanels();
            CloseMenu();
            StoryManager.Expand(transform.parent);
        }

        /// <summary>
        ///     Opens up the Info Panel on the Hexial Menu
        /// </summary>
        public void ListInfo()
        {
            InfoPanel.SetActive(!InfoPanel.activeSelf);
        }

        /// <summary>
        ///     Opens up the Enrich Panel on the Hexial Menu
        /// </summary>
        public void Enrich()
        {
            EnrichPanel.SetActive(!EnrichPanel.activeSelf);
        }

        /// <summary>
        ///     Opens up the Mitigate Panel on the Hexial Menu
        /// </summary>
        public void Mitigate()
        {
            MitigatePanel.SetActive(!MitigatePanel.activeSelf);
        }

        public void Back()
        {
            Debug.Log("Back");
        }

        /// <summary>
        ///     Resets the application back to the globe stage
        /// </summary>
        private void ResetStory()
        {
            StoryManager.ResetStory();
        }

        // Update is called once per frame
        private void Update()
        {
        }

        /// <summary>
        ///     Closes the hexial menu
        /// </summary>
        public void CloseMenu()
        {
            gameObject.SetActive(false);

            NetworkMessages.Instance.SendRadialMenu(-1, false);
        }

        /// <summary>
        ///     Closes all open Panels
        /// </summary>
        public void CloseAllPanels()
        {
            InfoPanel.SetActive(false);
            EnrichPanel.SetActive(false);
            MitigatePanel.SetActive(false);
        }

        public void UpdateRadialMenuClickIcon(NetworkInMessage msg)
        {
            var userId = msg.ReadInt64();
            var l = msg.ReadInt32();
            var methodNameChars = new char[l];
            for (var i = 0; i < l; ++i)
                methodNameChars[i] = Convert.ToChar(msg.ReadByte());
            var methodName = new string(methodNameChars);
            CloseMenu();
            Invoke(methodName, 0);
        }

        public void ChangeColor(Material color)
        {
            GetComponent<Renderer>().material = color;
        }

        [Serializable]
        public struct JNodeMenu
        {
            public NodeMenuItem[] nodeMenuItems;

            [Serializable]
            public struct NodeMenuItem
            {
                public string name;
                public string methodName;
                public string color;
                public string texture;
                public SubNodeMenuItem[] subNodeMenu;
            }

            [Serializable]
            public struct SubNodeMenuItem
            {
                public string actionName;
                public string actionValue;
            }
        }
    }
}