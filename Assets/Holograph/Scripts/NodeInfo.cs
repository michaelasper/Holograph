using System.Collections.Generic;

public class NodeInfo {


    public Dictionary<string, string> nodeDictionary;

    public NodeInfo(string nodeName, string nodeType, string[] keyList, string[] valueList)
    {
        if (keyList.Length != valueList.Length) throw new System.Exception("Invalid key-value list");


        nodeDictionary = new Dictionary<string, string>();
        nodeDictionary.Add("name", nodeName);
        nodeDictionary.Add("type", nodeType);

        for(int i = 0; i < keyList.Length; i++)
        {
            nodeDictionary.Add(keyList[i], valueList[i]);
        }
    }

   

    public void AddProperty(string propertyName, string propertyValue)
    {
        nodeDictionary.Add(propertyName, propertyValue);
    }

    public string GetProperty(string propertyName)
    {
        return nodeDictionary[propertyName];
    }

    public override string ToString()
    {
        return nodeDictionary.ToString();
    }

}
