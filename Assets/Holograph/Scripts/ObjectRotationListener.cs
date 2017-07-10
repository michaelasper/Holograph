using Holograph;
using HoloToolkit.Sharing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace holograph
{
    public class ObjectRotationListener : MonoBehaviour
    {

        private void Start()
        {
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.ObjectRotation] += updateRotation;
        }

        private void updateRotation(NetworkInMessage msg)
        {
            long userId = msg.ReadInt64();
            int objectId = msg.ReadInt32();
            float angle = msg.ReadFloat();

            if(GetInstanceID().Equals(objectId))
            {
                transform.Rotate(0, angle, 0, Space.World);
            }
        }
    }
}