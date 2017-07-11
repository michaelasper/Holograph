using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Holograph
{
    public class SpawnMenu : MonoBehaviour, IInputClickHandler
    {

        public GameObject RadialMenu;

        // Use this for initialization
        void Start()
        {


        }

        public void OnInputClicked(InputClickedEventData data)
        {


            if (RadialMenu.activeSelf)
            {
                if (RadialMenu.transform.parent == this.transform)
                {
                    RadialMenu.SetActive(false);
                }
                else
                {
                    RadialMenu.transform.parent = this.transform;
                }
            }
            else
            {
                if (RadialMenu.transform.parent != this.transform)
                {

                    RadialMenu.transform.parent = this.transform;
                }
                RadialMenu.SetActive(true);
            }
            RadialMenu.transform.localPosition = Vector3.zero;
            //RadialMenu.transform.localScale = Vector3.one * 2;
            //RadialMenu.transform.parent = this.transform.parent;
            //RadialMenu.SetActive(!RadialMenu.activeSelf);

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}