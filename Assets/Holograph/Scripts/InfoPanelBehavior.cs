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
            int numChildren = PropertyList.childCount;
            for (int i = 0; i < numChildren; ++i)
            {
                GameObject.Destroy(PropertyList.GetChild(0));
            }
            int numProps = nodeInfo.Count;
            for (int i = 0; i < numProps; ++i)
            {
                Instantiate(NodePropertyPrefab, PropertyList);
            }
            int k = 0;
            float prevY = 0f;
            foreach (KeyValuePair<string, string> p in nodeInfo)
            {
                Transform propertyTransform = PropertyList.GetChild(k);
                Transform keyObject = propertyTransform.GetChild(0);
                Transform valueObject = propertyTransform.GetChild(1);
                keyObject.GetComponent<Text>().text = p.Key;
                valueObject.GetComponent<Text>().text = p.Value;
                RectTransform propertyRectTransform = propertyTransform.GetComponent<RectTransform>();
                propertyRectTransform.sizeDelta = new Vector2(propertyRectTransform.sizeDelta.x, valueObject.GetComponent<RectTransform>().sizeDelta.y);
                ////propertyRectTransform.anchorMin = new Vector2(0, valueObject.GetComponent<RectTransform>().sizeDelta.y);
                propertyTransform.localPosition = new Vector3(0f, prevY, 0f);
                ++k;
                prevY -= propertyRectTransform.sizeDelta.y;
            }
        }
    }
}