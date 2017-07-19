using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using HoloToolkit.Sharing;

namespace Holograph
{
    public class SpawnMenu : MonoBehaviour, IInputHandler
    {

        public GameObject RadialMenu;

        private void Awake()
        {
            this.name = GetInstanceID().ToString();
        }

        // Use this for initialization
        void Start()
        {
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.RadialMenu] = UpdateRadialMenu;
        }

        public void OnInputDown(InputEventData eventData)
        {

        }

        private void OpenCloseMenu(bool isActiveSelf, bool isParentTransform)
        {
            if (isActiveSelf)
            {
                if (isParentTransform)
                {

                    RadialMenu.SetActive(false);
                }
                else
                {
                    RadialMenu.transform.parent = this.transform;
                }
            }
            else
            {
                if (!isParentTransform)
                {
                    RadialMenu.transform.parent = this.transform;
                }
                RadialMenu.SetActive(true);
            }
            RadialMenu.transform.localPosition = Vector3.zero;
        }

        public void OnInputUp(InputEventData data)
        {
            NetworkMessages.Instance.SendRadialMenu(GetComponent<NodeBehavior>().id, RadialMenu.activeSelf, RadialMenu.transform.parent == this.transform);
            OpenCloseMenu(RadialMenu.activeSelf, RadialMenu.transform.parent == this.transform);
        }

        private void UpdateRadialMenu(NetworkInMessage msg)
        {
            long userId = msg.ReadInt64();
            int id = msg.ReadInt32();
            bool isActiveSelf = Convert.ToBoolean(msg.ReadByte());
            bool isParentTransform = Convert.ToBoolean(msg.ReadByte());

            if(id == GetComponent<NodeBehavior>().id)
            {
                OpenCloseMenu(isActiveSelf, isParentTransform);
            }
        }
    }
}