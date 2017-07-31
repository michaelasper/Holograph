// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;

    using HoloToolkit.Sharing;

    using UnityEngine;

    using Random = UnityEngine.Random;

    /// <summary>
    ///     The map manager.
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        /// <summary>
        ///     The globe.
        /// </summary>
        public GameObject Globe;

        /// <summary>
        ///     The JSON File.
        /// </summary>
        public TextAsset JsonFile;

        /// <summary>
        /// List of node objects
        /// </summary>
        public GameObject[] NodeObject;

        /// <summary>
        ///     The node prefabs.
        /// </summary>
        public StringPrefabPair[] NodePrefabs;

        /// <summary>
        /// The hexagonal menu.
        /// </summary>
        public GameObject HexialMenu;

        /// <summary>
        /// List of the visibilities of all Nodes.
        /// </summary>
        public bool[] Visible;

        /// <summary>
        /// The adjacency matrix of the graph
        /// </summary>
        private int[,] adjMatrix;

        /// <summary>
        /// Dictionary from names to node IDs
        /// </summary>
        private Dictionary<string, int> nodeId;

        /// <summary>
        /// Hides all the Nodes
        /// </summary>
        public void HideNodes()
        {
            this.HexialMenu.transform.SetParent(transform.parent);
            this.HexialMenu.SetActive(false);
            foreach (GameObject t in this.NodeObject)
            {
                Destroy(t);
            }
        }

        /// <summary>
        /// Initializes the map
        /// </summary>
        /// <exception cref="FileNotFoundException">
        /// Thrown when JSON file is not specified (in Unity inspector)
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when JSON specify an unsupported node type
        /// </exception>
        public void InitMap()
        {
            if (this.JsonFile == null)
            {
                throw new FileNotFoundException("JSON not found");
            }

            string json = this.JsonFile.text;
            var jsonGraph = JsonUtility.FromJson<JGraph>(json);
            int numNodes = jsonGraph.Nodes.Length;
            this.adjMatrix = new int[numNodes, numNodes];
            this.NodeObject = new GameObject[numNodes];
            this.Visible = new bool[numNodes];
            this.nodeId = new Dictionary<string, int>();
            for (var i = 0; i < numNodes; ++i)
            {
                var nodeInfo = new NodeInfo(jsonGraph.Nodes[i].Name, jsonGraph.Nodes[i].Type, jsonGraph.Nodes[i].Keys, jsonGraph.Nodes[i].Values);
                var nodePrefab = (from stringPrefabPair in this.NodePrefabs where stringPrefabPair.NodeType.Equals(jsonGraph.Nodes[i].Type) select stringPrefabPair.NodePrefab)
                    .FirstOrDefault();
                if (nodePrefab == null)
                {
                    throw new NotSupportedException("JSON specifies unsupported node type");
                }

                var node = Instantiate(nodePrefab, transform);
                var nodebehvaior = node.GetComponent<NodeBehavior>();
                node.name = jsonGraph.Nodes[i].Name;
                nodebehvaior.SetNodeInfo(nodeInfo);
                nodebehvaior.id = i;
                this.nodeId.Add(jsonGraph.Nodes[i].Name, i);
                this.NodeObject[i] = node;
                node.SetActive(false);
            }

            for (var i = 0; i < jsonGraph.Edges.Length; ++i)
            {
                int sourceId = this.nodeId[jsonGraph.Edges[i].Source];
                int targetId = this.nodeId[jsonGraph.Edges[i].Target];
                this.adjMatrix[sourceId, targetId] = 1;
                this.adjMatrix[targetId, sourceId] = 1;
                var source = this.NodeObject[sourceId];
                var target = this.NodeObject[targetId];
                var sourceNeighborhood = source.GetComponent<NodeBehavior>().Neighborhood;
                var targetNeighborhood = target.GetComponent<NodeBehavior>().Neighborhood;
                sourceNeighborhood.AddLast(target);
                targetNeighborhood.AddLast(source);
            }

            this.Visible[0] = true;
            this.NodeObject[0].SetActive(true);
            this.Globe.GetComponent<GlobeBehavior>().firstNode = this.NodeObject[0];
        }

        /// <summary>
        /// Toggles the menu
        /// </summary>
        /// <param name="clickedNodeId">
        /// The node id.
        /// </param>
        public void TogglesMenu(int clickedNodeId)
        {
            var radialMenuParentId = this.HexialMenu.transform.GetComponentInParent<NodeBehavior>()?.id;
            if (clickedNodeId.Equals(radialMenuParentId))
            {
                this.HexialMenu.SetActive(!this.HexialMenu.activeSelf);
            }
            else
            {
                var targetNodeTransform = this.NodeObject[clickedNodeId].transform;
                this.HexialMenu.transform.SetParent(targetNodeTransform, false);
                this.HexialMenu.transform.localScale = Vector3.Scale(
                    new Vector3(.1f, .1f, .1f),
                    new Vector3(1f / targetNodeTransform.localScale.x, 1f / targetNodeTransform.localScale.y, 1f / targetNodeTransform.localScale.z));
                this.HexialMenu.SetActive(true);
            }

            this.HexialMenu.transform.localPosition = Vector3.zero;
            NetworkMessages.Instance.SendRadialMenu(clickedNodeId, this.HexialMenu.activeSelf);
        }

        /// <summary>
        /// Positions the Nodes using Fruchterman Reingold algorithm
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Proper noun for the algorithm")]
        public void PositionNodes()
        {
            var positions = this.SparseFruchtermanReingold(0);

            for (var i = 0; i < positions.Length; ++i)
            {
                this.NodeObject[i].GetComponent<NodeMovement>().moveTo(transform.TransformPoint(positions[i]));
            }
        }

        /// <summary>
        /// The sparse fruchterman reingold algorithm.
        /// </summary>
        /// <param name="originNode">
        /// The origin node.
        /// </param>
        /// <returns>
        /// The <see cref="Vector3[]"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Proper noun for the algorithm")]
        private Vector3[] SparseFruchtermanReingold(int originNode)
        {
            int numNodes = this.adjMatrix.GetLength(0);
            float k = Mathf.Sqrt(1f / numNodes);
            var t = .2f;
            var iterations = 75;
            float dt = t / iterations;
            var pos = new Vector3[numNodes];
            pos[originNode] = Vector3.zero;
            Random.InitState(123);
            for (var i = 0; i < numNodes; ++i)
            {
                if (i != originNode)
                {
                    pos[i] = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                }
            }

            for (var iteration = 0; iteration < iterations; ++iteration)
            {
                var displacement = new Vector3[numNodes];
                for (var i = 0; i < numNodes; ++i)
                {
                    if (!this.Visible[i] || i == originNode)
                    {
                        continue;
                    }

                    for (var j = 0; j < numNodes; ++j)
                    {
                        if (!this.Visible[j])
                        {
                            continue;
                        }

                        var delta = pos[i] - pos[j];
                        float dist = Vector3.Distance(pos[i], pos[j]);
                        dist = dist > 0.01f ? dist : 0.01f;
                        var force = delta * (((k * k) / (dist * dist)) - ((this.adjMatrix[i, j] * dist) / k));
                        displacement[i] += force;
                    }
                }

                for (var i = 0; i < numNodes; ++i)
                {
                    if (!this.Visible[i] || i == originNode)
                    {
                        continue;
                    }

                    pos[i] += displacement[i].normalized * t;
                }

                t -= dt;
            }

            return pos;
        }

        /// <summary>
        /// Start method called by Unity Engine
        /// </summary>
        private void Start()
        {
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.RadialMenu] = this.HandleMenuNetworkMessage;
        }

        /// <summary>
        /// Updates the menu around the node
        /// </summary>
        /// <param name="message">
        /// The network message
        /// </param>
        private void HandleMenuNetworkMessage(NetworkInMessage message)
        {
            message.ReadInt64();
            int clickedNodeId = message.ReadInt32();
            bool setActive = Convert.ToBoolean(message.ReadByte());
            var targetNodeTransform = this.NodeObject[clickedNodeId].transform;
            this.HexialMenu.transform.SetParent(targetNodeTransform, false);
            this.HexialMenu.transform.localScale = Vector3.Scale(
                new Vector3(.1f, .1f, .1f),
                new Vector3(1f / targetNodeTransform.localScale.x, 1f / targetNodeTransform.localScale.y, 1f / targetNodeTransform.localScale.z));
            this.HexialMenu.transform.localPosition = Vector3.zero;
            this.HexialMenu.SetActive(setActive);
        }

        /// <summary>
        /// The deserialized struct for the JSON File
        /// </summary>
        [Serializable]
        public struct JGraph
        {
            /// <summary>
            /// The Nodes.
            /// </summary>
            public Node[] Nodes;

            /// <summary>
            /// The Edges.
            /// </summary>
            public Edge[] Edges;

            /// <summary>
            /// Defines node struct
            /// </summary>
            [Serializable]
            public struct Node
            {
                /// <summary>
                /// The name of node. Unique
                /// </summary>
                public string Name;

                /// <summary>
                /// The node type.
                /// </summary>
                public string Type;

                /// <summary>
                /// The keys in property list.
                /// </summary>
                public string[] Keys;

                /// <summary>
                /// The values in property list.
                /// </summary>
                public string[] Values;
            }

            /// <summary>
            /// The definition of an edge.
            /// </summary>
            [Serializable]
            public struct Edge
            {
                /// <summary>
                /// The source of edge.
                /// </summary>
                public string Source;

                /// <summary>
                /// The target of edge.
                /// </summary>
                public string Target;
            }
        }

        /// <summary>
        /// The wrapper function for list of materials so that Unity is able to serialize in inspector
        /// </summary>
        [Serializable]
        public struct StringPrefabPair
        {
            /// <summary>
            /// The node type.
            /// </summary>
            public string NodeType;

            /// <summary>
            /// The node prefab.
            /// </summary>
            public GameObject NodePrefab;
        }
    }
}