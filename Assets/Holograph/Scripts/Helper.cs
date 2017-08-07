using Holograph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Holograph.Scripts
{
    public class Helper
    {
        public static MapManager.CaseList.Case.Node[] JsonObjectToNodeArray(JSONObject currentCase)
        {
            MapManager.CaseList.Case.Node[] results = new MapManager.CaseList.Case.Node[currentCase["Nodes"].Count];

            for(int i = 0; i < currentCase["Nodes"].Count; i++)
            {
                var currentNode = currentCase["Nodes"][i];
                var temp = new MapManager.CaseList.Case.Node();
                temp.Name = currentNode["Name"].ToString();
                temp.Type = currentNode["Type"].ToString();
                temp.Data = currentCase["Nodes"][i]["Data"].ToDictionary();
                temp.Keys = new string[0];
                temp.Values = new string[0];
                
                results[i] = temp;
            }

            return results;
        }

        public static MapManager.CaseList.Case.Edge[] JsonObjectToEdgeArray(JSONObject currentCase)
        {
            MapManager.CaseList.Case.Edge[] results = new MapManager.CaseList.Case.Edge[currentCase["Edges"].Count];

            for(int i = 0; i < currentCase["Edges"].Count; i++)
            {
                results[i] = new MapManager.CaseList.Case.Edge();
                results[i].Source = currentCase["Edges"][i]["Source"].ToString();
                results[i].Target = currentCase["Edges"][i]["Target"].ToString();
            }

            return results;
        }
    }
}
