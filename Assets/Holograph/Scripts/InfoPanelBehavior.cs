using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;


namespace Holograph {

    public class InfoPanelBehavior : MonoBehaviour {


        public TextMesh[] infoTextMeshes;
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
            int textMeshIndex = 0;
            foreach (var dictItem in info.nodeDictionary)
            {
                infoTextMeshes[textMeshIndex].text = dictItem.Key + ":  " + dictItem.Value;
                textMeshIndex++;

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