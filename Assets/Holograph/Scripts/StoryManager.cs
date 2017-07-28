using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public void ResetStory()
        {
            globe.gameObject.SetActive(true);
            globe.GetComponent<Collider>().enabled = true;
            globe.GetComponent<GlobeBehavior>().rotating = true;
            reportPanel.SetActive(true);
            if (globeAnimator != null && globeAnimator.isInitialized)
            {
                globeAnimator.SetTrigger(fadesInHash);
                //NetworkMessages.Instance.SendAnimationHash(fadesInHash, NetworkMessages.AnimationTypes.Trigger);
            }
            mapManager.hideNodes();
        }

        public void Expand(Transform expandedNode)
        {
            foreach (GameObject node in expandedNode.GetComponent<NodeBehavior>().neighborhood)
            {

                mapManager.visible[node.GetComponent<NodeBehavior>().id] = true;
                node.SetActive(true);
            }
            mapManager.positionNodes();
            //MenuAnimator.SetBool("Button_1", false);

            //CloseMenu();
            infoPanelBehavior.ClosePanel();
        }

        public void ListInfo(Transform node)
        {
            infoPanel.SetActive(true);
            NodeInfo nodeInfo = node.GetComponent<NodeBehavior>().nodeInfo;
            infoPanelBehavior.UpdateInfo(nodeInfo);
        }

        public void DefaultStoryEntry()
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
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}