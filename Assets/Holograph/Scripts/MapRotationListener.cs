using Holograph;
using HoloToolkit.Sharing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holograph
{
    public class MapRotationListener : MonoBehaviour
    {
        [Tooltip("Controls the speed at which the object will interpolate toward the desired rotation")]
        [Range(0.01f, 1.0f)]
        public float RotationLerpSpeed = 0.2f;

        public Quaternion targetRotation;
        void Start()
        {
            targetRotation = transform.rotation;
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.MapRotation] = updateRotation;
            NetworkMessages.Instance.SendMapRotation(transform.rotation);
        }
        void Update()
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationLerpSpeed);
        }

        private void updateRotation(NetworkInMessage msg)
        {
            long userId = msg.ReadInt64();
            targetRotation = new Quaternion(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
        }
    }
}