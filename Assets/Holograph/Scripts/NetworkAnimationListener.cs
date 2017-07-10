using HoloToolkit.Sharing;
using UnityEngine;

namespace Holograph
{
    [RequireComponent(typeof(Animator))]
    public class NetworkAnimationListener : MonoBehaviour
    {
        private Animator NetworkAnimator;
        private AnimatorControllerParameter[] animatorHashes;

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
        }

        private void UpdateAnimationHash(NetworkInMessage msg)
        {
            long userId = msg.ReadInt64();
            int animationHash = msg.ReadInt32();
            int animationType = msg.ReadInt32();
            float animationValue = msg.ReadFloat();

            Debug.Log("Message recived:");
            Debug.Log("    userId: " + userId);
            Debug.Log("    animationHash: " + animationHash);
            Debug.Log("    animationType: " + animationType);
            Debug.Log("    animationValue: " + animationValue);


            if (NetworkAnimator != null && NetworkAnimator.gameObject.activeInHierarchy)
            {
                if (animatorHashes == null)
                {
                    animatorHashes = NetworkAnimator.parameters;
                }

                for (int i = 0; i < animatorHashes.Length; i++)
                {
                    if (animatorHashes[i].nameHash == animationHash)
                    {
                        switch(animationType)
                        {
                            case (int)NetworkMessages.AnimationTypes.Boolean:
                                NetworkAnimator.SetBool(animationHash, !NetworkAnimator.GetBool(animationHash));
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
                            default:
                                Debug.Log("Animation message not matched to a handler");
                                break;
                        }
                    }
                }
            }
        }
    }
}