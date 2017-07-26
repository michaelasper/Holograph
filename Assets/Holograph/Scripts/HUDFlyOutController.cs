using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDFlyOutController : Singleton<HUDFlyOutController> {

    public GameObject Level1;
    public GameObject Level2;
    public GameObject Level3;

    public HUDFlyOutController()
    {

    }

    // Use this for initialization
    void Start () {
        Level1.SetActive(false);
        Level2.SetActive(false);
        Level3.SetActive(false);
    }

    public void ShowLevel1()
    {
        Level1.SetActive(true);
    }

    public void ShowLevel2()
    {
        Level2.SetActive(true);
    }

    public void ShowLevel3()
    {
        Level3.SetActive(true);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
