using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPanelBehavior : MonoBehaviour {


    public TextMesh info;
	// Use this for initialization
	void Start () {
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateInfo(string info)
    {
        this.info.text = info;
    }
}
