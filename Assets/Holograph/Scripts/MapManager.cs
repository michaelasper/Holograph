using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {


    public TextAsset jsonfile;

    public GameObject RadialMenu; 

    public GameObject NodeFab;

    private GameObject[] Slides;

    public Material[] materials;


	// Use this for initialization
	void Start () {

        Slides = new GameObject[this.transform.childCount];
        //Generates list of slides in the map
        for(int i = 0; i < Slides.Length; i++)
        {
            Slides[i] = this.transform.GetChild(i).gameObject;
            Debug.Log(Slides[i].name);
        }

        InitializeMap();
	}

    private Material MatchMaterial(string color)
    {
        for(int i = 0; i < materials.Length; i++)
        {
            if (materials[i].name == color) return materials[i];
        }

        throw new System.Exception("Invalid Material name: " + color);
    }
	

    void InitializeMap()
    {
        if (jsonfile == null) throw new System.Exception("JSON File not found");

        string json = jsonfile.text;
        var script = JsonUtility.FromJson<jGraph>(json);
        for (int i = 0; i < Slides.Length; i++)
        {
            int nodeCount = script.slides[i].count;
            Debug.Log(nodeCount);
            for(int j = 0; j < nodeCount; j++)
            {
                GameObject node = Instantiate(NodeFab, Slides[i].transform);
                NodeBehavior nodebehvaior = node.GetComponent<NodeBehavior>();
                
                node.GetComponent<SpawnMenu>().RadialMenu = this.RadialMenu;
                node.name = script.slides[i].nodes[j].name;
                nodebehvaior.ChangeName(node.name);
                Material material = MatchMaterial(script.slides[i].nodes[j].color);
                nodebehvaior.ChangeColor(material);
            }
        }
    }
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject[] GetSlides()
    {
        Debug.Log("Reached");
        return Slides;
    }

    private void GenerateLine(Vector3 start, Vector3 end)
    {


        return;
    }

    [System.Serializable]
    public struct jGraph
    {
        public Slide[] slides;
        [System.Serializable]
        public struct Slide
        {
            public string  name;
            public int count;

            public Node[] nodes;

            [System.Serializable]
            public struct Node
            {
                public string name;
                public string color;
            }
        }
        
    }
}
