using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holograph
{
    public class NodeMovement : MonoBehaviour
    {
        public GameObject globe;
        public float speedPercentage;
        public float maxSpeed;
        public Vector3 moveTarget;
        public float bounceOutAmt;
        
        void Start()
        {
            speedPercentage = 0f;
            bounceOutAmt = 0f;
        }
        
        void Update()
        {
            if (bounceOutAmt > 0.001f)
            {
                transform.position = (transform.position - globe.transform.position).normalized * (.25f + bounceOutAmt) + globe.transform.position;
            }
            if (speedPercentage > 0.001f)
            {
                float speed = maxSpeed * speedPercentage;
                transform.position = Vector3.MoveTowards(transform.position, moveTarget,
                    speed * Time.deltaTime);
            }
        }
    }
}