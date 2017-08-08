// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;
    using System.Collections.Generic;

    using HoloToolkit.Unity;

    using UnityEngine;
    using UnityEngine.UI;

    public class InfoPanelBehavior : MonoBehaviour
    {
        public Transform PropertyList;
        public GameObject NodePropertyPrefab;

        public void UpdateInfo(Dictionary<string, string> nodeInfo)
        {
            foreach (KeyValuePair<string, string> p in nodeInfo)
            {
                Transform propertyTransform = Instantiate(NodePropertyPrefab, PropertyList).transform;
                Transform keyObject = propertyTransform.GetChild(0);
                Transform valueObject = propertyTransform.GetChild(1);
                keyObject.GetComponent<Text>().text = p.Key;
                valueObject.GetComponent<Text>().text = p.Value;
            }
        }
    }
}