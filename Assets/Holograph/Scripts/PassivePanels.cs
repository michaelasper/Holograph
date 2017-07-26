using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassivePanels : MonoBehaviour {
    public Text Attendees;
    public Text Cases;
    public int Interval = 5;
    public GameObject Graph;

    private string _attendeeScan = "scanning...";
    private string _casesScan = "";
    private int _attendees = 19;
    private int _cases = 1768;
    private int _switcher = 1;

    //private GameObject _graph;

	// Use this for initialization
	void Start () {
        //_graph = GameObject.FindGameObjectWithTag("graph");
        //_graph.SetActive(false);
        Attendees.text = _attendeeScan;
        Cases.text = _casesScan;
        StartCoroutine(PanelLoop());		
	}

    private IEnumerator PanelLoop()
    {
        while(true)
        {
            ChangeText();
            yield return new WaitForSeconds(Interval);
        }
    }

    private void ChangeText()
    {
        if (Attendees.text  == _attendeeScan && Cases.text == _casesScan)
        {
            Attendees.text = ($"Attendees: {_attendees} \n   -Tayler \n   -Casey \n   -Ash \n   -Alex");
            Cases.text = ($"Active Cases: {_cases}");
            Graph.SetActive(true);
            if(_switcher % 2 == 0)
            {
                _cases = _cases - 3;
                _attendees = _attendees + 2;
            }
            else
            {
                _cases = _cases + 2;
                _attendees = _attendees - 1;
            }
        }
        else { 
            Attendees.text = _attendeeScan;
            Cases.text = _casesScan;
            Graph.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update () {
		
	}
}
