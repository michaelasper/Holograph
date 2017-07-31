using System;
using System.Collections.Generic;

namespace Holograph
{
    public class NodeInfo
    {
        public Dictionary<string, string> NodeDictionary;

        public NodeInfo(string nodeName, string nodeType, string[] keyList, string[] valueList)
        {
            if (keyList.Length != valueList.Length) throw new Exception("Invalid key-value list");


            NodeDictionary = new Dictionary<string, string> {{"name", nodeName}, {"type", nodeType}};

            for (var i = 0; i < keyList.Length; i++)
                NodeDictionary.Add(keyList[i], valueList[i]);
        }


        public void AddProperty(string propertyName, string propertyValue)
        {
            NodeDictionary.Add(propertyName, propertyValue);
        }

        public string GetProperty(string propertyName)
        {
            return NodeDictionary[propertyName];
        }

        public override string ToString()
        {
            return NodeDictionary.ToString();
        }
    }
}