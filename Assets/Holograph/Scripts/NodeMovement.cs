using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holograph
{
    public class NodeMovement : MonoBehaviour
    {

        public float speedPercentage;
        public float maxSpeed;
        public Vector3 moveTarget;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (speedPercentage > 0.01f)
            {
                float speed = maxSpeed * speedPercentage;
                transform.position = Vector3.MoveTowards(transform.position, moveTarget,
                    speed * Time.deltaTime);
            }
        }
    }
}