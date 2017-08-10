// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;
    using Assets.Holograph.Scripts;

    public class CaseObject
    {
        public int[,] adjMatrix;

        public Dictionary<string, int> nodeIndex;

        public GameObject[] NodeObject;

        public int numNodes;

        public MapManager.CaseList.Case.Node[] Nodes;
        public MapManager.CaseList.Case.Edge[] Edges;

        public bool[] Visible;

        public string Name;
        public int CaseId;

        public CaseObject(string name, int caseId, int numNodes)
        {
            this.Name = name.Replace("\"", "");
            this.CaseId = caseId;
            this.numNodes = numNodes;
            this.adjMatrix = new int[numNodes,numNodes];
            this.nodeIndex = new Dictionary<string, int>();
            this.NodeObject = new GameObject[numNodes];
            this.Visible = new bool[numNodes];
        }

        //public void SetUp(MapManager.CaseList.Case jsonGraph, MapManager.StringPrefabPair[] nodePrefabs)
        //{
        //    Debug.Log("Setting up: " + this.Name);
        //    this.Nodes = (MapManager.CaseList.Case.Node[])jsonGraph.Nodes.Clone();
        //    Debug.Log(this.Nodes);
        //    this.Edges = (MapManager.CaseList.Case.Edge[])jsonGraph.Edges.Clone();
        //    Debug.Log(this.Edges);

        //    for (var i = 0; i < this.numNodes; i++)
        //    {
        //        this.nodeIndex.Add(this.Nodes[i]._id, i);
               
        //    }


        //    for (var i = 0; i < this.Edges.Length; ++i)
        //    {
        //        int sourceId = this.nodeIndex[this.Edges[i].Source];
        //        int targetId = this.nodeIndex[this.Edges[i].Target];
        //        this.adjMatrix[sourceId, targetId] = 1;
        //        this.adjMatrix[targetId, sourceId] = 1;
                
        //    }
        //}

        public void SetUp(JSONObject jsonGraph)
        {
            this.Nodes = Helper.JsonObjectToNodeArray(jsonGraph);
            this.Edges = Helper.JsonObjectToEdgeArray(jsonGraph);

            for (var i = 0; i < this.numNodes; i++)
            {
                this.nodeIndex.Add(this.Nodes[i]._id, i);

            }


            for (var i = 0; i < this.Edges.Length; ++i)
            {
                int sourceIndex, targetIndex;
                try
                {
                    sourceIndex = this.nodeIndex[this.Edges[i].Source];
                }

                catch (KeyNotFoundException)
                {
                    throw new FormatException("Cannot find node " + this.Edges[i].Source);
                }

                try
                {
                    targetIndex = this.nodeIndex[this.Edges[i].Target];
                }

                catch (KeyNotFoundException)
                {
                    throw new FormatException("Cannot find node " + this.Edges[i].Target);
                }

                this.adjMatrix[sourceIndex, targetIndex] = 1;
                this.adjMatrix[targetIndex, sourceIndex] = 1;

            }
        }

        /// <summary>
        /// Hides the case
        /// </summary>
        public void HideCase()
        {
            foreach (GameObject t in this.NodeObject)
            {
                UnityEngine.Object.Destroy(t);
            }
        }
    }
}