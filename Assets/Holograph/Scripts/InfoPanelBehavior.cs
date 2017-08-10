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
                PropertyList.GetChild(i).gameObject.SetActive(false);
            }
            int k = 0;
            foreach (KeyValuePair<string, string> p in nodeInfo)
            {
                Transform propertyTransform;
                if (k < numChildren)
                {
                    propertyTransform = PropertyList.GetChild(k);
                    propertyTransform.gameObject.SetActive(true);
                }
                else
                {
                    propertyTransform = Instantiate(NodePropertyPrefab, PropertyList).transform;
                    propertyTransform.SetAsLastSibling();
                }
                ++k;
                Transform keyObject = propertyTransform.GetChild(0);
                Transform valueObject = propertyTransform.GetChild(1);
                keyObject.GetComponent<Text>().text = truncated(p.Key, 8);
                valueObject.GetComponent<Text>().text = truncated(p.Value, 23);
            }
            Transform panel = this.transform.GetChild(0).GetChild(0);
            RectTransform panelRectTransform = panel.GetComponent<RectTransform>();
            Debug.Log("Looking at " + panel.name);
            Debug.Log("Changing " + panelRectTransform.offsetMin);
            panelRectTransform.offsetMin = new Vector2(panelRectTransform.offsetMin.x, 45f - 12f * k);
            Debug.Log("to " + panelRectTransform.offsetMin);
        }

        private string truncated(string s, int l)
        {
            return s.Length <= l ? s : s.Substring(0, l) + "...";
        }
    }
}