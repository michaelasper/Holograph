using System;
using System.Collections.Generic;
using System.Linq;
using HoloToolkit.Sharing;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Holograph
{
    public class MapManager : MonoBehaviour
    {
        private int[,] adjMatrix;

        //public GameObject NodeFab;

        public GameObject globe;
        public TextAsset jsonfile;

        public Material[] materials;

        //nodes and their indices in the adj-matrix
        private Dictionary<string, int> nodeId;

        public GameObject[] nodeObject;

        public StringPrefabPair[] nodePrefabs;

        public GameObject RadialMenu;

        public bool[] visible;

        // Use this for initialization
        private void Start()
        {
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.RadialMenu] = UpdateRadialMenu;
        }

        private Material MatchMaterial(string color)

        {
            for (var i = 0; i < materials.Length; i++)
                if (materials[i].name == color) return materials[i];
            throw new Exception("Invalid Material name: " + color);
        }

        public void initMap()
        {
            if (jsonfile == null) throw new Exception("JSON File not found");

            var json = jsonfile.text;
            var jGraph = JsonUtility.FromJson<JGraph>(json);
            var numNodes = jGraph.nodes.Length;
            adjMatrix = new int[numNodes, numNodes];
            nodeObject = new GameObject[numNodes];
            visible = new bool[numNodes];
            nodeId = new Dictionary<string, int>();
            for (var i = 0; i < numNodes; ++i)
            {
                var nodeInfo = new NodeInfo(jGraph.nodes[i].name, jGraph.nodes[i].type, jGraph.nodes[i].keyList,
                    jGraph.nodes[i].valueList);
                var nodePrefab = (from stringPrefabPair in nodePrefabs where stringPrefabPair.nodeType.Equals(jGraph.nodes[i].type) select stringPrefabPair.nodePrefab).FirstOrDefault();
                if (nodePrefab == null)
                    throw new NotSupportedException("JSON specifies unsupported node type");
                var node = Instantiate(nodePrefab, transform);
                var nodebehvaior = node.GetComponent<NodeBehavior>();
                node.name = jGraph.nodes[i].name;
                nodebehvaior.SetNodeInfo(nodeInfo);
                //Material material = MatchMaterial(jGraph.nodes[i].color);
                //nodebehvaior.ChangeColor(material);
                nodebehvaior.id = i;
                nodeId.Add(jGraph.nodes[i].name, i);
                nodeObject[i] = node;
                node.SetActive(false);
            }
            for (var i = 0; i < jGraph.edges.Length; ++i)
            {
                var sourceId = nodeId[jGraph.edges[i].source];
                var targetId = nodeId[jGraph.edges[i].target];
                adjMatrix[sourceId, targetId] = 1;
                adjMatrix[targetId, sourceId] = 1;
                var source = nodeObject[sourceId];
                var target = nodeObject[targetId];
                var sourceNeighborhood = source.GetComponent<NodeBehavior>().Neighborhood;
                var targetNeighborhood = target.GetComponent<NodeBehavior>().Neighborhood;
                sourceNeighborhood.AddLast(target);
                targetNeighborhood.AddLast(source);
            }
            visible[0] = true;
            nodeObject[0].SetActive(true);
            globe.GetComponent<GlobeBehavior>().firstNode = nodeObject[0];
        }

        public void positionNodes()
        {
            var positions = sparseFruchtermanReingold(0);
            if (positions == null)
                throw new NullReferenceException("Fruchterman-Reingold algorithm returns null");
            for (var i = 0; i < positions.Length; ++i)
                nodeObject[i].GetComponent<NodeMovement>().moveTo(transform.TransformPoint(positions[i]));
        }

        private Vector3[] sparseFruchtermanReingold(int originNode)
        {
            var numNodes = adjMatrix.GetLength(0);
            var k = Mathf.Sqrt(1f / numNodes);
            var t = .2f;
            var iterations = 75;
            var dt = t / iterations;
            var pos = new Vector3[numNodes];
            pos[originNode] = Vector3.zero;
            Random.InitState(123);
            for (var i = 0; i < numNodes; ++i)
                if (i != originNode)
                    pos[i] = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            for (var iteration = 0; iteration < iterations; ++iteration)
            {
                var displacement = new Vector3[numNodes];
                for (var i = 0; i < numNodes; ++i)
                {
                    if (!visible[i] || i == originNode)
                        continue;
                    for (var j = 0; j < numNodes; ++j)
                    {
                        if (!visible[j])
                            continue;
                        var delta = pos[i] - pos[j];
                        var dist = Vector3.Distance(pos[i], pos[j]);
                        dist = dist > 0.01f ? dist : 0.01f;
                        var force = delta * (k * k / (dist * dist) - adjMatrix[i, j] * dist / k);
                        displacement[i] += force;
                    }
                }
                for (var i = 0; i < numNodes; ++i)
                {
                    if (!visible[i] || i == originNode)
                        continue;
                    pos[i] += displacement[i].normalized * t;
                }
                t -= dt;
            }
            return pos;
        }

        private void Update()
        {
        }

        public void hideNodes()
        {
            RadialMenu.transform.SetParent(transform.parent);
            RadialMenu.SetActive(false);
            for (var i = 0; i < nodeObject.Length; ++i)
                Destroy(nodeObject[i]);
        }

        public void menuClickedOn(int nodeId)
        {
            var radialMenuParentId = RadialMenu.transform.GetComponentInParent<NodeBehavior>()?.id;
            if (nodeId.Equals(radialMenuParentId))
            {
                RadialMenu.SetActive(!RadialMenu.activeSelf);
            }
            else
            {
                var targetNodeTransform = nodeObject[nodeId].transform;
                RadialMenu.transform.SetParent(targetNodeTransform, false);
                RadialMenu.transform.localScale = Vector3.Scale(new Vector3(.1f, .1f, .1f),
                    new Vector3(1f / targetNodeTransform.localScale.x, 1f / targetNodeTransform.localScale.y,
                        1f / targetNodeTransform.localScale.z));
                RadialMenu.SetActive(true);
            }
            RadialMenu.transform.localPosition = Vector3.zero;
            NetworkMessages.Instance.SendRadialMenu(nodeId, RadialMenu.activeSelf);
        }

        private void UpdateRadialMenu(NetworkInMessage msg)
        {
            var userId = msg.ReadInt64();
            var nodeId = msg.ReadInt32();
            var setActive = Convert.ToBoolean(msg.ReadByte());
            var targetNodeTransform = nodeObject[nodeId].transform;
            RadialMenu.transform.SetParent(targetNodeTransform, false);
            RadialMenu.transform.localScale = Vector3.Scale(new Vector3(.1f, .1f, .1f),
                new Vector3(1f / targetNodeTransform.localScale.x, 1f / targetNodeTransform.localScale.y,
                    1f / targetNodeTransform.localScale.z));
            RadialMenu.transform.localPosition = Vector3.zero;
            RadialMenu.SetActive(setActive);
        }

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

        [Serializable]
        public struct JGraph
        {
            public Node[] nodes;
            public Edge[] edges;

            [Serializable]
            public struct Node
            {
                public string name;
                public string type;
                public string[] keyList;
                public string[] valueList;
            }

            [Serializable]
            public struct Edge
            {
                public string source;
                public string target;
            }
        }
    }
}