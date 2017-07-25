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
        private RadialMenuManager.JGraph.Nodemenuitem[] NodeMenuItemArray;  

        //private GameObject[] ObjectList = new GameObject[4];
        public GameObject IconFab;
        public GameObject Map;
        public GameObject Globe;
        public GameObject ReportPanel;
        //private GameObject[] Slides;

        private InfoPanelBehavior infoPanelBehavior;
        public Animator GraphAnimator;
        public Animator MenuAnimator;
        private Animator globeAnimator;

        private MapManager mapManager;
        public GameObject InfoPanel;

        private int fadesInHash;

        // Use this for initialization
        void Start()
        {
            // Starts with Menu open to run Start() Script
            CloseMenu();
            MenuAnimator = this.GetComponent<Animator>();
            NodeMenuItemArray = GetComponent<RadialMenuManager>().jGraph.nodeMenuItems;
            //Debug.Log(NodeMenuItemArray);
            for (int i = 0; i < NodeMenuItemArray.Length; i++)
            {
                GameObject icon = this.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).gameObject;
                icon.name = NodeMenuItemArray[i].name;
                icon.GetComponent<Icon>().menubehvaior = this;
                icon.GetComponent<Icon>().TextureName = NodeMenuItemArray[i].texture;
                //for(int j = 0; j < NodeMenuItemArray[i].subNodeMenu.Length; j++)
                //{
                    //icon.GetComponent<Icon>().Message = NodeMenuItemArray[i].subNodeMenu[j].actionName;
                //}
            }

            mapManager = Map.GetComponent<MapManager>();
            infoPanelBehavior = InfoPanel.GetComponent<InfoPanelBehavior>();
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.RadialMenuClickIcon] = UpdateRadialMenuClickIcon;
            globeAnimator = Globe.GetComponent<Animator>();
            fadesInHash = Animator.StringToHash("fadesIn");
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
            MenuAnimator.SetBool("Button_1", false);

            CloseMenu();
            infoPanelBehavior.ClosePanel();
        }


        public void ListInfo()
        {


            InfoPanel.SetActive(true);

            NodeInfo nodeInfo = this.transform.parent.GetComponent<NodeBehavior>().nodeInfo;
            infoPanelBehavior.UpdateInfo(nodeInfo);
        }

        public void Hack1()
        {
            Debug.Log("Hack1");
            resetStory();
        }

        public void Hack2()
        {
            Debug.Log("Hack2");
        }

        private void resetStory()
        {
            Globe.SetActive(true);
            Globe.GetComponent<Collider>().enabled = true;
            Globe.GetComponent<GlobeBehavior>().rotating = true;
            ReportPanel.SetActive(true);
            if (globeAnimator != null && globeAnimator.isInitialized)
            {
                globeAnimator.SetTrigger(fadesInHash);
                //NetworkMessages.Instance.SendAnimationHash(fadesInHash, NetworkMessages.AnimationTypes.Trigger);
            }
            mapManager.hideNodes();
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
            Invoke(methodName, 0);
        }
        public void ChangeColor(Material color)
        {
            GetComponent<Renderer>().material = color;
        }


    }
}