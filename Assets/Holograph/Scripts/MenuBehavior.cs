using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holograph
{
    public class MenuBehavior : MonoBehaviour
    {
        private string[] TextureList = { "First Icon", "Second Icon", "Third Icon", "Fourth Icon" };
        private string[] ActionList = { "Expand", "ListInfo", "Hack1", "Hack2" };


        private GameObject[] ObjectList = new GameObject[4];
        public GameObject IconFab;
        public GameObject Map;
        private GameObject[] Slides;

        public Animator GraphAnimator;
        public Animator MenuAnimator;
        private int stageCount = 0;
        private MapManager mapManager;


        // Use this for initialization
        void Start()
        {
            this.gameObject.SetActive(false);
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
            //Slides =  mapManager.GetSlides();


        }

        public int GetSlideLocation()
        {
            GameObject parent = this.transform.parent.parent.gameObject;
            Debug.Log(parent.name);
            //if (Slides == null) Slides = mapManager.GetSlides();
            for (int i = 0; i < Slides.Length; i++)
            {
                if (parent.Equals(Slides[i])) return i;
            }

            return -1;
        }


        /// <summary>
        /// Expands the graph when icon is hit
        /// </summary>
        public void Expand()
        {
            foreach (GameObject node in this.transform.parent.GetComponent<NodeBehavior>().neighborhood)
            {
                MapManager mapManager = node.transform.parent.GetComponent<MapManager>();
                mapManager.visible[node.GetComponent<NodeBehavior>().id] = true;
                node.SetActive(true);
                mapManager.positionNodes();
            }
            MenuAnimator.SetBool("Button_1", false);
            this.gameObject.SetActive(false);


        }

        private void ConnectNodes(int SlideLocation)
        {
            //GameObject slide = Slides[SlideLocation];
            //GameObject StartNode = this.transform.parent.gameObject;

            //int ChildCount = slide.transform.childCount;
            //for (int i = 0; i < ChildCount; i++)
            //{
            //    NodeBehavior Node = slide.transform.GetChild(i).GetComponent<NodeBehavior>();
            //    //Node.EnableLines(StartNode, i, SlideLocation);

            //}
            //Debug.Log("Nodes Connected");

        }

        public void ListInfo()
        {
            Debug.Log("Hack");
        }

        public void Hack1()
        {
            Debug.Log("Hack1");
        }

        public void Hack2()
        {
            Debug.Log("Hack2");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}