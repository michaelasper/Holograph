using UnityEngine;

namespace Holograph
{
    public class NodeMovement : MonoBehaviour
    {
        private float _moveMaxSpeed;

        private readonly int _movesHash = Animator.StringToHash("moves");
        private Vector3 _moveTarget;
        public float SpeedPercentage;

        private void Start()
        {
            SpeedPercentage = 0f;
        }

        private void Update()
        {
            if (SpeedPercentage > 0.001f)
            {
                var speed = _moveMaxSpeed * SpeedPercentage;
                transform.position = Vector3.MoveTowards(transform.position, _moveTarget,
                    speed * Time.deltaTime);
            }
        }

        public void moveTo(Vector3 target)
        {

            _moveTarget = target;
            _moveMaxSpeed = Vector3.Distance(transform.position, target);
            var nodeAnimator = GetComponent<Animator>();
            var id = GetComponent<NodeBehavior>().id;
            if (nodeAnimator == null || !nodeAnimator.isInitialized) return;
            nodeAnimator.SetTrigger(_movesHash);
            NetworkMessages.Instance.SendAnimationHash(_movesHash, NetworkMessages.AnimationTypes.Trigger);
        }
    }
}