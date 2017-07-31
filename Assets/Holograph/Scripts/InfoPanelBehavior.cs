// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;

    using HoloToolkit.Unity;

    using UnityEngine;
    using UnityEngine.UI;

    public class InfoPanelBehavior : MonoBehaviour
    {
        public NodeInfo info;

        public Text[] InfoTextList;

        public bool isTaggedToUser = true;

        /// <summary>
        ///     Will close the panel if the panel is not pinned
        ///     Returns: state of panel
        /// </summary>
        public bool ClosePanel()
        {
            if (isTaggedToUser)
            {
                gameObject.SetActive(false);
            }

            return gameObject.activeSelf;
        }

        public void ForceClosePane()
        {
            gameObject.SetActive(false);
        }

        public void PinPanel()
        {
            isTaggedToUser = !isTaggedToUser;
            GetComponentInChildren<Tagalong>().enabled = isTaggedToUser;
        }

        public void UpdateInfo(NodeInfo info)
        {
            this.info = info;
            UpdateText();
        }

        // Use this for initialization
        private void Start()
        {
            gameObject.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void UpdateText()
        {
            var textIndex = 0;
            foreach (var dictItem in info.NodeDictionary)
            {
                InfoTextList[textIndex].text = dictItem.Value;
                textIndex++;
            }
        }
    }
}