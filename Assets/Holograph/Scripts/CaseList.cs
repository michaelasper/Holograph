namespace Holograph
{
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    public class CaseList : MonoBehaviour
    {

        public GameObject CaseListItemPrefab;

        public StoryManager StoryManagerObject;

        public void SetUp(List<CaseObject> caseList)
        {
            for (var index = 0; index < caseList.Count; ++index)
            {
                var caseObject = caseList[index];
                string name = caseObject.Name;
                var caseItem = Instantiate(CaseListItemPrefab, this.gameObject.transform);
                caseItem.GetComponentInChildren<Text>().text = name;
                caseItem.GetComponent<CaseListButton>().CaseID = index;
                caseItem.GetComponent<CaseListButton>().StoryManagerObject = StoryManagerObject;
            }
        }
    }
}