// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;

    using HoloToolkit.Unity.InputModule;

    using UnityEngine;
    using UnityEngine.UI;

    public class ListHoverBehavior : MonoBehaviour, IFocusable
    {
        private Text listItemText;

        public void OnFocusEnter()
        {
            this.listItemText.color = Color.blue;
        }

        public void OnFocusExit()
        {
            this.listItemText.color = Color.white;

        }

        void Start()
        {
            this.listItemText = gameObject.GetComponent<Text>();
            
        }
    }
}