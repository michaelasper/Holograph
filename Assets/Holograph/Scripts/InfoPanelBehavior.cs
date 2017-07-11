using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPanelBehavior : MonoBehaviour {


    public TextMesh[] infoTextMeshes;
    public NodeInfo info;
	// Use this for initialization
	void Start () {
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateInfo(NodeInfo info)
    {
        this.info = info;
        UpdateText();
    }

    private void UpdateText()
    {
        int textMeshIndex = 0;
        foreach(var dictItem in info.nodeDictionary)
        {
            infoTextMeshes[textMeshIndex].text = dictItem.Key + ":\t" + dictItem.Value;
            textMeshIndex++;

        }
    }
}
