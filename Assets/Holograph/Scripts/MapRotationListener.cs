// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;

    using HoloToolkit.Sharing;

    using UnityEngine;

    public class MapRotationListener : MonoBehaviour
    {
        [Tooltip("Controls the speed at which the object will interpolate toward the desired rotation")]
        [Range(0.01f, 1.0f)]
        public float RotationLerpSpeed = 0.2f;

        public Quaternion targetRotation;

        private void Start()
        {
            targetRotation = transform.rotation;
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.MapRotation] = updateRotation;
            NetworkMessages.Instance.SendMapRotation(transform.rotation);
        }

        private void Update()
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