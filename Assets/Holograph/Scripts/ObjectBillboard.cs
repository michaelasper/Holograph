using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("Not used anymore", true)]

public class ObjectBillboard : MonoBehaviour
{
    private Transform cam;
	// Use this for initialization
	void Start ()
	{
	    cam = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(cam);
	}
}
