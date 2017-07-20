using Holograph;
using HoloToolkit.Sharing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holograph
{
    public class ObjectRotationListener : MonoBehaviour
    {
        [Tooltip("Controls the speed at which the object will interpolate toward the desired rotation")]
        [Range(0.01f, 1.0f)]
        public float RotationLerpSpeed = 0.2f;

        private int instanceId;
        public Quaternion targetRotation;
        void Start()
        {
            targetRotation = transform.rotation;
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.ObjectRotation] = updateRotation;
            instanceId = GetInstanceID();
            NetworkMessages.Instance.SendObjectRotation(instanceId, transform.rotation);
        }
        void Update()
        {
            Debug.Log("updating");
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationLerpSpeed);
        }

        private void updateRotation(NetworkInMessage msg)
        {
            long userId = msg.ReadInt64();
            int objectId = msg.ReadInt32();

            if (instanceId.Equals(objectId))
            {
                targetRotation = new Quaternion(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
            }
        }
    }
}