// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

using Holograph;

using HoloToolkit.Unity.InputModule;

using UnityEngine;

public class ActivateTestAnimationButton : MonoBehaviour, IInputClickHandler
{
    private AnimatorControllerParameter[] animatorHashes;

    private Animator NetworkAnimator;

    private int testBoolId;

    private int testTriggerId;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (NetworkAnimator != null && NetworkAnimator.gameObject.activeInHierarchy)
        {
            if (animatorHashes == null)
            {
                animatorHashes = NetworkAnimator.parameters;
            }

            for (var i = 0; i < animatorHashes.Length; i++)
            {
                if (animatorHashes[i].nameHash == testBoolId)
                {
                    NetworkAnimator.SetBool(testBoolId, !NetworkAnimator.GetBool(testBoolId));
                    NetworkMessages.Instance.SendAnimationHash(testBoolId, NetworkMessages.AnimationTypes.Boolean, !NetworkAnimator.GetBool(testBoolId) ? 1 : 0);
                }

                if (animatorHashes[i].nameHash == testTriggerId && NetworkAnimator.GetBool(testBoolId))
                {
                    NetworkAnimator.SetBool(testTriggerId, true);
                    NetworkMessages.Instance.SendAnimationHash(testTriggerId, NetworkMessages.AnimationTypes.Trigger);
                }
            }
        }
    }

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
        NetworkAnimator = GetComponent<Animator>();
    }
}