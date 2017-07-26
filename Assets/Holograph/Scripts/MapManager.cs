using System;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Sharing;

namespace Holograph
{
    public class MapManager : MonoBehaviour
    {
        public TextAsset jsonfile;
     
        public GameObject RadialMenu;

        //public GameObject NodeFab;

        public GameObject globe;

        public Material[] materials;

        public bool[] visible;

        //public GameObject alertNodePrefab;
        //public GameObject monitorNodePrefab;
        //public GameObject serverNodePrefab;
        //public GameObject userNodePrefab;

        [Serializable]
        public struct StringPrefabPair
        {
            public string nodeType;
            public GameObject nodePrefab;
        }

        public StringPrefabPair[] nodePrefabs;

        //nodes and their indices in the adj-matrix
        private Dictionary<string, int> nodeId;

        private GameObject[] nodeObject;

        private int[,] adjMatrix;

        // Use this for initialization
        void Start()
        {
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.RadialMenu] = UpdateRadialMenu;
        }

        private Material MatchMaterial(string color)

        {
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].name == color) return materials[i];
            }
            throw new System.Exception("Invalid Material name: " + color);
        }

        public void initMap()
        {
            if (jsonfile == null) throw new System.Exception("JSON File not found");

            string json = jsonfile.text;
            var jGraph = JsonUtility.FromJson<JGraph>(json);
            int numNodes = jGraph.nodes.Length;
            adjMatrix = new int[numNodes, numNodes];
            nodeObject = new GameObject[numNodes];
            visible = new bool[numNodes];
            nodeId = new Dictionary<string, int>();
            for (int i = 0; i < numNodes; ++i)
            {
                NodeInfo nodeInfo = new NodeInfo(jGraph.nodes[i].name, jGraph.nodes[i].type, jGraph.nodes[i].keyList, jGraph.nodes[i].valueList);
                GameObject nodePrefab = null;
                foreach (StringPrefabPair stringPrefabPair in nodePrefabs)
                {
                    if (stringPrefabPair.nodeType.Equals(jGraph.nodes[i].type))
                    {
                        nodePrefab = stringPrefabPair.nodePrefab;
                        break;
                    }
                }
                //switch (jGraph.nodes[i].type)
                //{
                //    case "ALERT":
                //        nodePrefab = alertNodePrefab;
                //        break;
                //    case "MONITOR":
                //        nodePrefab = monitorNodePrefab;
                //        break;
                //    case "SERVER":
                //        nodePrefab = serverNodePrefab;
                //        break;
                //    case "USER":
                //        nodePrefab = userNodePrefab;
                //        break;
                //    default:
                //        nodePrefab = null;
                //        break;
                //}
                if (nodePrefab == null)
                {
                    throw new NotSupportedException("JSON specifies unsupported node type");
                }
                GameObject node = Instantiate(nodePrefab, this.transform);
                NodeBehavior nodebehvaior = node.GetComponent<NodeBehavior>();
                node.name = jGraph.nodes[i].name;
                nodebehvaior.SetNodeInfo(nodeInfo);
                //Material material = MatchMaterial(jGraph.nodes[i].color);
                //nodebehvaior.ChangeColor(material);
                nodebehvaior.id = i;
                nodeId.Add(jGraph.nodes[i].name, i);
                nodeObject[i] = node;
                node.SetActive(false);
            }
            for (int i = 0; i < jGraph.edges.Length; ++i)
            {
                int sourceId = nodeId[jGraph.edges[i].source];
                int targetId = nodeId[jGraph.edges[i].target];
                adjMatrix[sourceId, targetId] = 1;
                adjMatrix[targetId, sourceId] = 1;
                GameObject source = nodeObject[sourceId];
                GameObject target = nodeObject[targetId];
                LinkedList<GameObject> sourceNeighborhood = source.GetComponent<NodeBehavior>().neighborhood;
                LinkedList<GameObject> targetNeighborhood = target.GetComponent<NodeBehavior>().neighborhood;
                sourceNeighborhood.AddLast(target);
                targetNeighborhood.AddLast(source);
            }
            visible[0] = true;
            nodeObject[0].SetActive(true);
            globe.GetComponent<GlobeBehavior>().firstNode = nodeObject[0];
        }

        public void positionNodes()
        {
            Vector3[] positions = sparseFruchtermanReingold(0);
            if (positions == null)
            {
                throw new System.NullReferenceException("Fruchterman-Reingold algorithm returns null");
            }
            for (int i = 0; i < positions.Length; ++i)
            {
                nodeObject[i].GetComponent<NodeMovement>().moveTo(transform.TransformPoint(positions[i]));
            }
        }

        private Vector3[] sparseFruchtermanReingold(int originNode)
        {
            int numNodes = adjMatrix.GetLength(0);
            float k = Mathf.Sqrt(1f / numNodes);
            float t = .2f;
            int iterations = 75;
            float dt = t / iterations;
            Vector3[] pos = new Vector3[numNodes];
            pos[originNode] = Vector3.zero;
            UnityEngine.Random.InitState(123);
            for (int i = 0; i < numNodes; ++i)
            {
                if (i != originNode)
                {
                    pos[i] = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
                }
            }
            for (int iteration = 0; iteration < iterations; ++iteration)
            {
                Vector3[] displacement = new Vector3[numNodes];
                for (int i = 0; i < numNodes; ++i)
                {
                    if (!visible[i] || i == originNode)
                    {
                        continue;
                    }
                    for (int j = 0; j < numNodes; ++j)
                    {
                        if (!visible[j])
                        {
                            continue;
                        }
                        Vector3 delta = pos[i] - pos[j];
                        float dist = Vector3.Distance(pos[i], pos[j]);
                        dist = dist > 0.01f ? dist : 0.01f;
                        Vector3 force = delta * ((k * k) / (dist * dist) - adjMatrix[i, j] * dist / k);
                        displacement[i] += force;
                    }
                }
                for (int i = 0; i < numNodes; ++i)
                {
                    if (!visible[i] || i == originNode)
                    {
                        continue;
                    }
                    pos[i] += displacement[i].normalized * t;
                }
                t -= dt;
            }
            return pos;
        }

        void Update()
        {

        }

        public void hideNodes()
        {
            RadialMenu.transform.parent = this.transform.parent;
            RadialMenu.SetActive(false);
            for (int i = 0; i < nodeObject.Length; ++i)
            {
                GameObject.Destroy(nodeObject[i]);
            }
        }

        public void menuClickedOn(int nodeId)
        {
            int? radialMenuParentId = RadialMenu.transform.parent.GetComponent<NodeBehavior>()?.id;
            if (nodeId.Equals(radialMenuParentId))
            {
                RadialMenu.SetActive(!RadialMenu.activeSelf);
            }
            else
            {
                Debug.Log("got it");
                RadialMenu.transform.parent = nodeObject[nodeId].transform;
                RadialMenu.SetActive(true);
            }
            RadialMenu.transform.localPosition = Vector3.zero;
            NetworkMessages.Instance.SendRadialMenu(nodeId, RadialMenu.activeSelf);
        }

        private void UpdateRadialMenu(NetworkInMessage msg)
        {
            long userId = msg.ReadInt64();
            int nodeId = msg.ReadInt32();
            bool setActive = Convert.ToBoolean(msg.ReadByte());
            RadialMenu.transform.parent = nodeObject[nodeId].transform;
            RadialMenu.transform.localPosition = Vector3.zero;
            RadialMenu.SetActive(setActive);
        }

        [System.Serializable]
        public struct JGraph
        {

            public Node[] nodes;
            public Edge[] edges;

            [System.Serializable]
            public struct Node
            {
                public string name;
                public string type;
                public string[] keyList;
                public string[] valueList;
            }
            [System.Serializable]
            public struct Edge
            {
                public string source;
                public string target;
            }
        }
    }
}