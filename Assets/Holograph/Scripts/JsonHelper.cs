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
    using System.Text;
    using System.Threading.Tasks;

    public class JsonHelper
    {
        public static MapManager.CaseList.Case.Node[] JsonObjectToNodeArray(JSONObject currentCase)
        {
            MapManager.CaseList.Case.Node[] nodes = new MapManager.CaseList.Case.Node[currentCase["Nodes"].Count];
            for (int i = 0; i < currentCase["Nodes"].Count; ++i)
            {
                JSONObject currentNode = currentCase["Nodes"][i];
                nodes[i] = new MapManager.CaseList.Case.Node()
                {
                    Name = currentNode["Name"].ToString().Replace("\"", ""),
                    Type = currentNode["Type"].ToString().Replace("\"", ""),
                    _id = currentNode["_id"].ToString().Replace("\"", ""),
                    Data = currentNode["Data"].ToDictionary()
                };
            }

            return nodes;
        }

        public static MapManager.CaseList.Case.Edge[] JsonObjectToEdgeArray(JSONObject currentCase)
        {
            MapManager.CaseList.Case.Edge[] edges = new MapManager.CaseList.Case.Edge[currentCase["Edges"].Count];
            for (int i = 0; i < currentCase["Edges"].Count; i++)
            {
                JSONObject currentEdge = currentCase["Edges"][i];
                edges[i] = new MapManager.CaseList.Case.Edge()
                {
                    Source = currentEdge["Source"].ToString().Replace("\"", ""),
                    Target = currentEdge["Target"].ToString().Replace("\"", "")
                };
            }

            return edges;
        }

    }

}