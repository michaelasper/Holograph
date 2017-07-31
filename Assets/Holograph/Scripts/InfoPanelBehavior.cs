using System;
using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Holograph
{
    public class InfoPanelBehavior : MonoBehaviour
    {
        public NodeInfo info;


        public Text[] InfoTextList;

        public bool isTaggedToUser = true;

        // Use this for initialization
        private void Start()
        {
            gameObject.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void UpdateInfo(NodeInfo info)
        {
            this.info = info;
            UpdateText();
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

        public void ForceClosePane()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        ///     Will close the panel if the panel is not pinned
        ///     Returns: state of panel
        /// </summary>
        public bool ClosePanel()
        {
            if (isTaggedToUser) gameObject.SetActive(false);

            return gameObject.activeSelf;
        }

        public void PinPanel()
        {
            isTaggedToUser = !isTaggedToUser;
            GetComponentInChildren<Tagalong>().enabled = isTaggedToUser;
        }
    }
}