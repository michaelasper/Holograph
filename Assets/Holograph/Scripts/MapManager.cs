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
        ///     Audio source from default cursor for nodes to use.
        /// </summary>
        public AudioSource AudioSource;

        /// <summary>
        ///     The globe.
        /// </summary>
        public GameObject Globe;

        /// <summary>
        ///     The hexagonal menu.
        /// </summary>
        public GameObject HexialMenu;

        /// <summary>
        ///     The JSON File.
        /// </summary>
        public TextAsset JsonFile;

        public StoryManager MainStoryManager;

        /// <summary>
        ///     Audio clip for toggling off the node menu
        /// </summary>
        public AudioClip MenuOffSound;

        /// <summary>
        ///     Audio clip for toggling on the node menu
        /// </summary>
        public AudioClip MenuOnSound;

        /// <summary>
        ///     List of node objects
        /// </summary>
        public GameObject[] NodeObject;

        /// <summary>
        ///     The node prefabs.
        /// </summary>
        public StringPrefabPair[] NodePrefabs;


        public List<CaseObject> caseObjects;

        private bool caseLoaded;

        public int currentCase;


        public CaseObject getCurrentCaseObject()
        {
            return caseObjects.FirstOrDefault(caseObject => caseObject.CaseID == currentCase);
        }


        /// <summary>
        ///     Hides all the Nodes
        /// </summary>
        public void HideMap()
        {
            if (!caseLoaded)
            {
                return;
            }

            var targetCaseObject = caseObjects.FirstOrDefault(caseObject => caseObject.CaseID == currentCase);

            if (targetCaseObject == null)
            {
                throw new NullReferenceException("Case not found");
            }

            HexialMenu.transform.SetParent(transform.parent);
            HexialMenu.SetActive(false);
            foreach (var t in targetCaseObject.NodeObject)
            {
                Destroy(t);
            }

            caseLoaded = false;
        }

        /// <summary>
        ///     Hides all the Nodes
        /// </summary>
        [System.Obsolete]
        public void HideNodes()
        {
            HexialMenu.transform.SetParent(transform.parent);
            HexialMenu.SetActive(false);
            foreach (var t in NodeObject)
            {
                Destroy(t);
            }
        }

        /// <summary>
        ///     Initializes the map
        /// </summary>
        /// <exception cref="FileNotFoundException">
        ///     Thrown when JSON file is not specified (in Unity inspector)
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     Thrown when JSON specify an unsupported node type
        /// </exception>
        public void InitMap()
        {
            if (JsonFile == null)
            {
                throw new FileNotFoundException("JSON not found");
            }

            caseObjects = new List<CaseObject>();
            string json = JsonFile.text;
            var caseList = JsonUtility.FromJson<CaseList>(json);

            for (var index = 0; index < caseList.Cases.Length; index++)
            {
                var jsonGraph = caseList.Cases[index];
                int numNodes = jsonGraph.Nodes.Length;
                var caseObject = new CaseObject(jsonGraph.Name, index, numNodes);
                caseObject.SetUp(jsonGraph, NodePrefabs);
                caseObjects.Add(caseObject);
            }
        }

        /// <summary>
        ///     Loads a case
        /// </summary>
        /// <param name="caseId">
        ///     The name of the case which to load
        /// </param>
        public void LoadMap(int caseId)
        {
            if (caseLoaded) HideMap();

            var targetCaseObject = caseObjects.FirstOrDefault(caseObject => caseObject.CaseID == caseId);

            if (targetCaseObject == null)
            {
                throw new NullReferenceException("Case not found");
            }

            currentCase = caseId;

            for (var i = 0; i < targetCaseObject.numNodes; i++)
            {
                var nodeInfo = new NodeInfo(targetCaseObject.Nodes[i].Name, targetCaseObject.Nodes[i].Type, targetCaseObject.Nodes[i].Keys, targetCaseObject.Nodes[i].Values);
                var nodePrefab = (from stringPrefabPair in NodePrefabs
                                  where stringPrefabPair.NodeType.Equals(targetCaseObject.Nodes[i].Type)
                                  select stringPrefabPair.NodePrefab).FirstOrDefault();

                var node = Instantiate(nodePrefab, transform);
                var nodebehvaior = node.GetComponent<NodeBehavior>();
                node.name = targetCaseObject.Nodes[i].Name;
                nodebehvaior.SetNodeInfo(nodeInfo);
                nodebehvaior.id = i;
                targetCaseObject.NodeObject[i] = node;
                node.SetActive(false);
            }


            for (var i = 0; i < targetCaseObject.Edges.Length; ++i)
            {
                int sourceId = targetCaseObject.nodeId[targetCaseObject.Edges[i].Source];
                int targetId = targetCaseObject.nodeId[targetCaseObject.Edges[i].Target];
                var source = targetCaseObject.NodeObject[sourceId];
                var target = targetCaseObject.NodeObject[targetId];
                var sourceNeighborhood = source.GetComponent<NodeBehavior>().Neighborhood;
                var targetNeighborhood = target.GetComponent<NodeBehavior>().Neighborhood;
                sourceNeighborhood.AddLast(target);
                targetNeighborhood.AddLast(source);
            }
            caseLoaded = true;

            targetCaseObject.Visible[0] = true;
            targetCaseObject.NodeObject[0].SetActive(true);
            Globe.GetComponent<GlobeBehavior>().firstNode = targetCaseObject.NodeObject[0];
        }

        /// <summary>
        ///     Positions the Nodes using Fruchterman Reingold algorithm
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Proper noun for the algorithm")]
        public void PositionNodes()
        {
            var positions = SparseFruchtermanReingold(0);

            for (var i = 0; i < positions.Length; ++i)
            {
                getCurrentCaseObject().NodeObject[i].GetComponent<NodeMovement>().moveTo(transform.TransformPoint(positions[i]));
            }
        }

        /// <summary>
        ///     Toggles the menu
        /// </summary>
        /// <param name="clickedNodeId">
        ///     The node id.
        /// </param>
        public void TogglesMenu(int clickedNodeId)
        {
            var radialMenuParentId = HexialMenu.transform.GetComponentInParent<NodeBehavior>()?.id;
            if (clickedNodeId.Equals(radialMenuParentId))
            {
                HexialMenu.SetActive(!HexialMenu.activeSelf);
            }
            else
            {
                var currentCaseObject = getCurrentCaseObject();
                var targetNodeTransform = currentCaseObject.NodeObject[clickedNodeId].transform;
                HexialMenu.transform.SetParent(targetNodeTransform, false);
                HexialMenu.transform.localScale = Vector3.Scale(
                    new Vector3(.1f, .1f, .1f),
                    new Vector3(1f / targetNodeTransform.localScale.x, 1f / targetNodeTransform.localScale.y, 1f / targetNodeTransform.localScale.z));
                HexialMenu.SetActive(true);
            }

            AudioSource.PlayOneShot(HexialMenu.activeSelf ? MenuOnSound : MenuOffSound);
            HexialMenu.transform.localPosition = Vector3.zero;
            NetworkMessages.Instance.SendRadialMenu(clickedNodeId, HexialMenu.activeSelf);
        }

        /// <summary>
        ///     Updates the menu around the node
        /// </summary>
        /// <param name="message">
        ///     The network message
        /// </param>
        private void HandleMenuNetworkMessage(NetworkInMessage message)
        {
            message.ReadInt64();
            int clickedNodeId = message.ReadInt32();
            bool setActive = Convert.ToBoolean(message.ReadByte());
            var targetNodeTransform = NodeObject[clickedNodeId].transform;
            HexialMenu.transform.SetParent(targetNodeTransform, false);
            HexialMenu.transform.localScale = Vector3.Scale(
                new Vector3(.1f, .1f, .1f),
                new Vector3(1f / targetNodeTransform.localScale.x, 1f / targetNodeTransform.localScale.y, 1f / targetNodeTransform.localScale.z));
            HexialMenu.transform.localPosition = Vector3.zero;
            HexialMenu.SetActive(setActive);
        }

        /// <summary>
        ///     The sparse fruchterman reingold algorithm.
        /// </summary>
        /// <param name="originNode">
        ///     The origin node.
        /// </param>
        /// <returns>
        ///     The <see cref="Vector3[]" />.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Proper noun for the algorithm")]
        private Vector3[] SparseFruchtermanReingold(int originNode)
        {
            var targetCaseObject = caseObjects.FirstOrDefault(caseObject => caseObject.CaseID == currentCase);
            int numNodes = targetCaseObject.adjMatrix.GetLength(0);
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
                    if (!targetCaseObject.Visible[i] || i == originNode)
                    {
                        continue;
                    }

                    for (var j = 0; j < numNodes; ++j)
                    {
                        if (!targetCaseObject.Visible[j])
                        {
                            continue;
                        }

                        var delta = pos[i] - pos[j];
                        float dist = Vector3.Distance(pos[i], pos[j]);
                        dist = dist > 0.01f ? dist : 0.01f;
                        var force = delta * (k * k / (dist * dist) - targetCaseObject.adjMatrix[i, j] * dist / k);
                        displacement[i] += force;
                    }
                }

                for (var i = 0; i < numNodes; ++i)
                {
                    if (!targetCaseObject.Visible[i] || i == originNode)
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
        ///     Start method called by Unity Engine
        /// </summary>
        private void Start()
        {
            InitMap();
            caseLoaded = false;
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.RadialMenu] = HandleMenuNetworkMessage;
        }

        [Serializable]
        public struct CaseList
        {
            public Case[] Cases;

            /// <summary>
            ///     The deserialized struct for the JSON File
            /// </summary>
            [Serializable]
            public struct Case
            {
                public string Name;

                /// <summary>
                ///     The Nodes.
                /// </summary>
                public Node[] Nodes;

                /// <summary>
                ///     The Edges.
                /// </summary>
                public Edge[] Edges;

                /// <summary>
                ///     Defines node struct
                /// </summary>
                [Serializable]
                public struct Node
                {
                    /// <summary>
                    ///     The name of node. Unique
                    /// </summary>
                    public string Name;

                    /// <summary>
                    ///     The node type.
                    /// </summary>
                    public string Type;

                    /// <summary>
                    ///     The keys in property list.
                    /// </summary>
                    public string[] Keys;

                    /// <summary>
                    ///     The values in property list.
                    /// </summary>
                    public string[] Values;
                }

                /// <summary>
                ///     The definition of an edge.
                /// </summary>
                [Serializable]
                public struct Edge
                {
                    /// <summary>
                    ///     The source of edge.
                    /// </summary>
                    public string Source;

                    /// <summary>
                    ///     The target of edge.
                    /// </summary>
                    public string Target;
                }
            }
        }

        /// <summary>
        ///     The wrapper function for list of materials so that Unity is able to serialize in inspector
        /// </summary>
        [Serializable]
        public struct StringPrefabPair
        {
            /// <summary>
            ///     The node type.
            /// </summary>
            public string NodeType;

            /// <summary>
            ///     The node prefab.
            /// </summary>
            public GameObject NodePrefab;
        }
    }
}