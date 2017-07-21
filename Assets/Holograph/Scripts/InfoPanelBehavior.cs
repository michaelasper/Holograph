using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using UnityEngine.UI;

namespace Holograph {

    public class InfoPanelBehavior : MonoBehaviour {


        public Text[] infoTextList;
        public NodeInfo info;
        public bool isTaggedToUser = true;
        // Use this for initialization
        void Start() {
            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update() {

        }

        public void UpdateInfo(NodeInfo info)
        {
            this.info = info;
            UpdateText();
        }

        private void UpdateText()
        {
            int textIndex = 0;
            foreach (var dictItem in info.nodeDictionary)
            {
                infoTextList[textIndex].text = dictItem.Value;
                textIndex++;

            }
        }

        public void ForceClosePane()
        {
            gameObject.SetActive(false);
            
        }

        /// <summary>
        /// Will close the panel if the panel is not pinned
        /// 
        /// Returns: state of panel
        /// </summary>
        public bool ClosePanel()
        {
            if(isTaggedToUser) gameObject.SetActive(false);

            return gameObject.activeSelf;

        }

        public void PinPanel()
        {
            isTaggedToUser = !isTaggedToUser;
            this.GetComponentInChildren<Tagalong>().enabled = isTaggedToUser;
        }
        

    }
}