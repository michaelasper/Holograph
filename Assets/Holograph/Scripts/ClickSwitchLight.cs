using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

namespace Holograph
{
    public class ClickSwitchLight : MonoBehaviour, IInputHandler
    {
        public GameObject lightSource;
        void IInputHandler.OnInputDown(InputEventData eventData)
        {
            throw new NotImplementedException();
        }

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            lightSource.SetActive(!lightSource.activeSelf);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}