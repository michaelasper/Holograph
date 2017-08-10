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
    public class NetworkMenuAnimationListener : MonoBehaviour
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
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.MenuAnimationHash] = UpdateAnimationHash;
            NetworkAnimator = GetComponent<Animator>();
        }

        private void UpdateAnimationHash(NetworkInMessage msg)
        {
            msg.ReadInt64();
            int animationHash = msg.ReadInt32();
            int animationType = msg.ReadInt32();
            float animationValue = msg.ReadFloat();

            if (NetworkAnimator != null)
            {
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