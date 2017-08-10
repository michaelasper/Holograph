// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;

    using UnityEngine;
    public class NodeMovement : MonoBehaviour
    {
        public float SpeedPercentage;

        private readonly int _movesHash = Animator.StringToHash("moves");

        private float _moveMaxSpeed;

        private Vector3 _moveTarget;

        public void moveTo(Vector3 target)
        {
            _moveTarget = target;
            _moveMaxSpeed = Vector3.Distance(transform.position, target);
            var nodeAnimator = GetComponent<Animator>();
            int id = GetComponent<NodeBehavior>().Index;
            if (nodeAnimator == null || !nodeAnimator.isInitialized)
            {
                return;
            }

            nodeAnimator.SetTrigger(_movesHash);
            NetworkMessages.Instance.SendAnimationHash(_movesHash, NetworkMessages.AnimationTypes.Trigger);
        }

        private void Start()
        {
            SpeedPercentage = 0f;
        }

        private void Update()
        {
            if (SpeedPercentage > 0.001f)
            {
                float speed = _moveMaxSpeed * SpeedPercentage;
                transform.position = Vector3.MoveTowards(transform.position, _moveTarget, speed * Time.deltaTime);
            }

        }

    }

}