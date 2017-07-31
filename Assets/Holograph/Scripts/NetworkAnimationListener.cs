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

    [RequireComponent(typeof(Animator))]
    public class NetworkAnimationListener : MonoBehaviour
    {
        private AnimatorControllerParameter[] animatorHashes;

        private Animator NetworkAnimator;

        private int testBoolId;

        private int testTriggerId;

        private void Awake()
        {
            if (testBoolId == 0)
            {
                testBoolId = Animator.StringToHash("TestBool");
            }

            if (testTriggerId == 0)
            {
                testTriggerId = Animator.StringToHash("TestTrigger");
            }
        }

        private void Start()
        {
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.AnimationHash] = UpdateAnimationHash;

            NetworkAnimator = GetComponent<Animator>();

            Debug.Log("NetworkAnimator is null: " + NetworkAnimator == null);
        }

        private void UpdateAnimationHash(NetworkInMessage msg)
        {
            Debug.Log("UpdateAnimationHash() is called");

            // if (NetworkAnimator == null)
            // {
            // throw System.NullReferenceException("shit is null yo");
            // }
            long userId = msg.ReadInt64();
            int animationHash = msg.ReadInt32();
            int animationType = msg.ReadInt32();
            float animationValue = msg.ReadFloat();

            Debug.Log("Message recived:");
            Debug.Log("    userId: " + userId);
            Debug.Log("    animationHash: " + animationHash);
            Debug.Log("    animationType: " + animationType);
            Debug.Log("    animationValue: " + animationValue);

            if (NetworkAnimator != null)
            {
                // && NetworkAnimator.gameObject.activeInHierarchy)
                if (animatorHashes == null)
                {
                    animatorHashes = NetworkAnimator.parameters;
                }

                for (var i = 0; i < animatorHashes.Length; i++)
                {
                    if (animatorHashes[i].nameHash == animationHash)
                    {
                        switch (animationType)
                        {
                            case (int)NetworkMessages.AnimationTypes.Boolean:
                                NetworkAnimator.SetBool(animationHash, animationValue >= 0.5);
                                break;
                            case (int)NetworkMessages.AnimationTypes.Integer:
                                NetworkAnimator.SetInteger(animationHash, (int)animationValue);
                                break;
                            case (int)NetworkMessages.AnimationTypes.Float:
                                NetworkAnimator.SetFloat(animationHash, animationValue);
                                break;
                            case (int)NetworkMessages.AnimationTypes.Trigger:
                                NetworkAnimator.SetTrigger(animationHash);
                                break;
                            default: break;
                        }
                    }
                }
            }
        }
    }
}