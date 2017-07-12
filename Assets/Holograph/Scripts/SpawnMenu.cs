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

        public void OnInputUp(InputEventData data)
        {
            if (RadialMenu.activeSelf)
            {
                if (RadialMenu.transform.parent == this.transform)
                {
                    NetworkMessages.Instance.SendRadialMenu(this.transform, true, true);
                    RadialMenu.SetActive(false);
                }
                else
                {
                    NetworkMessages.Instance.SendRadialMenu(this.transform, true, false);
                    RadialMenu.transform.parent = this.transform;
                }
            }
            else
            {
                if (RadialMenu.transform.parent != this.transform)
                {
                    NetworkMessages.Instance.SendRadialMenu(this.transform, false, false);
                    RadialMenu.transform.parent = this.transform;
                }
                RadialMenu.SetActive(true);
            }
            RadialMenu.transform.localPosition = Vector3.zero;
            //RadialMenu.transform.localScale = Vector3.one * 2;
            //RadialMenu.transform.parent = this.transform.parent;
            //RadialMenu.SetActive(!RadialMenu.activeSelf);

        }

        private void UpdateRadialMenu(NetworkInMessage msg)
        {
            long userId = msg.ReadInt64();
            Vector3 position = NetworkMessages.Instance.ReadVector3(msg);
            Quaternion rotation = NetworkMessages.Instance.ReadQuaternion(msg);
            bool isActiveSelf = Convert.ToBoolean(msg.ReadByte());
            bool isParentTransform = Convert.ToBoolean(msg.ReadByte());


            if (isActiveSelf)
            {
                if (isParentTransform)
                {
                    RadialMenu.SetActive(false);
                }
                else
                {
                    RadialMenu.transform.parent.position = position;
                    RadialMenu.transform.parent.rotation = rotation;
                }
            }
            else
            {
                if (!isParentTransform)
                {
                    RadialMenu.transform.parent.position = position;
                    RadialMenu.transform.parent.rotation = rotation;
                }
                RadialMenu.SetActive(true);
            }
            RadialMenu.transform.localPosition = Vector3.zero;
        }
    }
}