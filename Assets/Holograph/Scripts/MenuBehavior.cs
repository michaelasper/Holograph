using Assets.Holograph;
using HoloToolkit.Sharing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holograph
{
    public class MenuBehavior : MonoBehaviour
    {
        // List of how Icons should look and what they should do
        private string[] TextureList = { "First Icon", "Second Icon", "Third Icon", "Fourth Icon" };
        private string[] ActionList = { "Expand", "ListInfo", "Hack1", "Hack2" };


        //private GameObject[] ObjectList = new GameObject[4];
        public GameObject IconFab;
        public GameObject Map;
        //private GameObject[] Slides;

        private InfoPanelBehavior infoPanelBehavior;
        public Animator GraphAnimator;
        public Animator MenuAnimator;

        private MapManager mapManager;
        public GameObject InfoPanel;

        // Use this for initialization
        void Start()
        {
            // Starts with Menu open to run Start() Script
            CloseMenu();
            MenuAnimator = this.GetComponent<Animator>();
            for (int i = 0; i < TextureList.Length; i++)
            {
                GameObject icon = this.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).gameObject;
                icon.name = TextureList[i];
                icon.GetComponent<Icon>().menubehvaior = this;
                icon.GetComponent<Icon>().TextureName = TextureList[i];
                icon.GetComponent<Icon>().Message = ActionList[i];

            }

            mapManager = Map.GetComponent<MapManager>();
            infoPanelBehavior = InfoPanel.GetComponent<InfoPanelBehavior>();

            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.RadialMenuState] = UpdateRadialMenuState;

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
                mapManager.positionNodes();
            }
            MenuAnimator.SetBool("Button_1", false);
            CloseMenu();
            infoPanelBehavior.ClosePanel();

            NetworkMessages.Instance.SendRadialMenuState("Expand", Helpers.GameObjectLinkListToArray(this.transform.parent.GetComponent<NodeBehavior>().neighborhood));
        }

        public void ListInfo()
        {


            InfoPanel.SetActive(true);

            NodeInfo nodeInfo = this.transform.parent.GetComponent<NodeBehavior>().nodeInfo;
            infoPanelBehavior.UpdateInfo(nodeInfo);

            NetworkMessages.Instance.SendRadialMenuState("ListInfo");
        }

        public void Hack1()
        {
            Debug.Log("Hack1");

            NetworkMessages.Instance.SendRadialMenuState("Hack1");
        }

        public void Hack2()
        {
            Debug.Log("Hack2");

            NetworkMessages.Instance.SendRadialMenuState("Hack2");
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void CloseMenu()
        {
            gameObject.SetActive(false);

            NetworkMessages.Instance.SendRadialMenuState("CloseMenu");
        }

        private void UpdateRadialMenuState(NetworkInMessage msg)
        {
            long userId = msg.ReadInt64();
            string action = msg.ReadString();

            switch(action)
            {
                case "Expand":
                    foreach (GameObject node in this.transform.parent.GetComponent<NodeBehavior>().neighborhood)
                    {

                        mapManager.visible[node.GetComponent<NodeBehavior>().id] = true;
                        node.SetActive(true);
                        mapManager.positionNodes();
                    }
                    MenuAnimator.SetBool("Button_1", false);
                    CloseMenu();
                    infoPanelBehavior.ClosePanel();

                    break;
                case "ListInfo":
                    InfoPanel.SetActive(true);

                    NodeInfo nodeInfo = this.transform.parent.GetComponent<NodeBehavior>().nodeInfo;
                    infoPanelBehavior.UpdateInfo(nodeInfo);

                    break;
                case "Hack1":
                    break;
                case "Hack2":
                    break;
                case "CloseMenu":
                    break;
                default:
                    Debug.Log("Recieved RadialMenuState but no handler was found");
                    break;
            }
        }
    }
}