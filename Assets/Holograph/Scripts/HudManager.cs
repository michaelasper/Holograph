using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holograph
{
    public class HudManager : MonoBehaviour
    {
        void Start()
        {

        }

        void Update()
        {

        }
        
        public void selectButton(Transform clickedButton)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform childButton = transform.GetChild(i);
                childButton.GetComponent<HudButtonBehavior>().switchSelected(clickedButton == childButton);
            }
        }
    }
}