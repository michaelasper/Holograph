using System.Collections.Generic;

public class NodeInfo {


    private Dictionary<string, string> nodeDictionary;

    public NodeInfo(string nodeName, string nodeType)
    {
        nodeDictionary = new Dictionary<string, string>();
        nodeDictionary.Add("name", nodeName);
        nodeDictionary.Add("type", nodeType);
    }

    public NodeInfo(string nodeName, string nodeType, Dictionary<string,string> nodeDictionary)
    {
        this.nodeDictionary = nodeDictionary;
        this.nodeDictionary.Add("name", nodeName);
        this.nodeDictionary.Add("type", nodeType);
    }

    public void AddProperty(string propertyName, string propertyValue)
    {
        nodeDictionary.Add(propertyName, propertyValue);
    }

    public string GetProperty(string propertyName)
    {
        return nodeDictionary[propertyName];
    }
  
}
