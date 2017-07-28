using HoloToolkit.Sharing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holograph
{
    public class MenuBehavior : MonoBehaviour
    {
        // List of how Icons should look and what they should do
        // private string[] TextureList = { "First Icon", "Second Icon", "Third Icon", "Fourth Icon" };
        //private string[] ActionList; //= { "Expand", "ListInfo", "Hack1", "Hack2" };

        //private GameObject[] ObjectList = new GameObject[4];
        //public GameObject IconFab;
        public StoryManager storyManager;
        //public GameObject Map;
        public GameObject Globe;
        public GameObject ReportPanel;
        //private GameObject[] Slides;

        private InfoPanelBehavior infoPanelBehavior;

        private MapManager mapManager;
        public GameObject InfoPanel;
        public TextAsset jsonfile;

        
        //void Start()
        void Awake()
        {
            //CloseMenu();
            //MenuAnimator = this.GetComponent<Animator>();
            if (jsonfile == null) throw new System.Exception("JSON File not found");

            string json = jsonfile.text;
            JNodeMenu.NodeMenuItem[] nodeMenuItems = JsonUtility.FromJson<JNodeMenu>(json).nodeMenuItems;
            Debug.Log("there are " + nodeMenuItems.Length + " menu items");
            for (int i = 0; i < nodeMenuItems.Length; i++)
            {
                //Debug.Log("we have node menu item " + nodeMenuItems[i]);
                transform.GetChild(i).GetComponent<ButtonBehavior>().initLayout(nodeMenuItems[i]);
                //Debug.Log("init " + transform.GetChild(i).GetComponent<ButtonBehavior>().methodName);
                //GameObject icon = this.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).gameObject;
                //icon.name = nodeMenuItems[i].name;
                //icon.GetComponent<Icon>().menubehvaior = this;
                //icon.GetComponent<Icon>().TextureName = nodeMenuItems[i].texture;
                //for(int j = 0; j < NodeMenuItemArray[i].subNodeMenu.Length; j++)
                //{
                    //icon.GetComponent<Icon>().Message = NodeMenuItemArray[i].subNodeMenu[j].actionName;
                //}
            }
            
            infoPanelBehavior = InfoPanel.GetComponent<InfoPanelBehavior>();
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.RadialMenuClickIcon] = UpdateRadialMenuClickIcon;
        }

        /// <summary>
        /// Expands the graph when icon is hit
        /// </summary>
        public void Expand()
        {
            foreach (GameObject node in this.transform.parent.GetComponent<NodeBehavior>().neighborhood)
            {

                mapManager.visible[node.GetComponent<NodeBehavior>().id] = true;
                node.SetActive(true);
            }
            mapManager.positionNodes();
            //MenuAnimator.SetBool("Button_1", false);

            //CloseMenu();
            infoPanelBehavior.ClosePanel();
        }


        public void ListInfo()
        {


            InfoPanel.SetActive(true);

            NodeInfo nodeInfo = this.transform.parent.GetComponent<NodeBehavior>().nodeInfo;
            infoPanelBehavior.UpdateInfo(nodeInfo);
        }

        public void Hack()
        {
            Debug.Log("Hack!");
        }

        public void Hack2()
        {
            Debug.Log("Hack2");
        }

        private void ResetStory()
        {
            storyManager.ResetStory();
        }
        // Update is called once per frame
        void Update()
        {

        }

        public void CloseMenu()
        {
            gameObject.SetActive(false);

            NetworkMessages.Instance.SendRadialMenu(-1, false);
        }

        public void UpdateRadialMenuClickIcon(NetworkInMessage msg)
        {
            long userId = msg.ReadInt64();
            int l = msg.ReadInt32();
            char[] methodNameChars = new char[l];
            for (int i = 0; i < l; ++i)
            {
                methodNameChars[i] = Convert.ToChar(msg.ReadByte());
            }
            string methodName = new string(methodNameChars);
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