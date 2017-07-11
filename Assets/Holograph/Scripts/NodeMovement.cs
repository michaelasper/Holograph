using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holograph
{
    public class NodeMovement : MonoBehaviour
    {
        //public GameObject globe;
        //public Animation moveAnimation;
        public float speedPercentage;
        private float moveMaxSpeed;
        private Vector3 moveTarget;
        //public float bounceOutAmt;

        private int movesHash = Animator.StringToHash("moves");

        void Start()
        {
            speedPercentage = 0f;
            //bounceOutAmt = 0f;
        }

        void Update()
        {
            //if (bounceOutAmt > 0.001f)
            //{
            //    transform.position = (transform.position - globe.transform.position).normalized * (.25f + bounceOutAmt) + globe.transform.position;
            //}
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