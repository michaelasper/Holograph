using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ReportPanelBehavior : MonoBehaviour {

    public RectTransform progressBar;
    public Text percentTextLabel;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var progress = progressBar.sizeDelta.x;
        var progressFraction = progress / .25f;
        percentTextLabel.text = ((int)(progressFraction * 100)).ToString() + "%";
	}
}
