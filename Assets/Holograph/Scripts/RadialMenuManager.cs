using System;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Sharing;

namespace Holograph
{
    public class RadialMenuManager : MonoBehaviour
    {
        public TextAsset jsonfile;

        public GameObject nodeMenu;

        public JGraph jGraph;

        // Use this for initialization
        void Start()
        {
           // Debug.Log("startingmenu RadialMenuManager");
            initNodeObject();
        }

        // Update is called once per frame
        void Update()
        {

        }
        [Serializable]
        public struct JGraph
        {
            public Nodemenuitem[] nodeMenuItems;

            [Serializable]
            public struct Nodemenuitem
            {
                public string name;
                public string color;
                public string texture;
                public Subnodemenu[] subNodeMenu;
            }

            [Serializable]
            public struct Subnodemenu
            {
                public string actionName;
                public string actionValue;
            }

        }

        public void initNodeObject()
        {
            //Debug.Log("starting init RadialMenuManager");
            if (jsonfile == null) throw new System.Exception("JSON File not found");

            string json = jsonfile.text;
            jGraph = JsonUtility.FromJson<JGraph>(json);
            //Debug.Log(jGraph.nodeMenuItems[0].name);
        }
    }
}
