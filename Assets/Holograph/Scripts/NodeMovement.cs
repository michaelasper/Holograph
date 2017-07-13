using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holograph
{
    public class NodeMovement : MonoBehaviour
    {
        public float speedPercentage;
        private float moveMaxSpeed;
        private Vector3 moveTarget;

        private int movesHash = Animator.StringToHash("moves");

        void Start()
        {
            speedPercentage = 0f;
        }

        void Update()
        {
            if (speedPercentage > 0.001f)
            {
                float speed = moveMaxSpeed * speedPercentage;
                transform.position = Vector3.MoveTowards(transform.position, moveTarget,
                    speed * Time.deltaTime);
            }
        }

        public void moveTo(Vector3 target)
        {
            this.moveTarget = target;
            this.moveMaxSpeed = Vector3.Distance(this.transform.position, target);
            Animator nodeAnimator = GetComponent<Animator>();
            int id = GetComponent<NodeBehavior>().id;
            if (nodeAnimator != null && nodeAnimator.isInitialized)
            {
                nodeAnimator.SetTrigger(movesHash);
                NetworkMessages.Instance.SendAnimationHash(movesHash, NetworkMessages.AnimationTypes.Trigger);
            }
        }
    }
}