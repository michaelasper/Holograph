// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;
    using System.Collections.Generic;
    [Obsolete]
    public class NodeInfo
    {
        public Dictionary<string, string> NodeData;

        public NodeInfo(string nodeName, string nodeType, Dictionary<string, string> nodeDataList)
        {
            NodeData = new Dictionary<string, string>(nodeDataList);
            NodeData.Add("name", nodeName);
            NodeData.Add("type", nodeType);
        }

        public void AddProperty(string propertyName, string propertyValue)
        {
            NodeData.Add(propertyName, propertyValue);
        }

        public string GetProperty(string propertyName)
        {
            return NodeData[propertyName];
        }

        public override string ToString()
        {
            return NodeData.ToString();
        }
    }
}