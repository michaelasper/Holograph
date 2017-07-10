using System;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Holograph
{
    public class GlobeBehavior : MonoBehaviour, IInputHandler
    {

        public GameObject globe;
        public GameObject node;

        public float rotSpeed = 20f;
        
        public Animator globeAnimator;
        public Animator nodeAnimator;
        private int sceneNum;
        public bool pushPinMode;
        public Transform pushPin;
        public bool rotating;

        public List<Vector3> pinPositions;

        private int invisibleStateHash;
        private int appearsHash;
        private int fadesOutHash;
        private int showsCircleHash;

        private Transform cam;

        void IInputHandler.OnInputDown(InputEventData eventData)
        {
            RaycastHit hit;
            Ray ray = new Ray(cam.position, cam.forward);
            if (Physics.Raycast(ray, out hit) && hit.transform.name == "Globe")
            {
                GlobeCircleWave circleWave = GetComponent<GlobeCircleWave>();
                //circleWave.showCircle = true;
                circleWave.initCircleLines(hit.point);
                //globeAnimator.SetTrigger(showsCircleHash);
            }
        }

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            AirTap();
        }

        void Start()
        {
            sceneNum = 1;
            pinPositions = new List<Vector3>();
            invisibleStateHash = Animator.StringToHash("Base Layer.Invisible");
            appearsHash = Animator.StringToHash("appears");
            fadesOutHash = Animator.StringToHash("fadesOut");
            showsCircleHash = Animator.StringToHash("showsCircle");
            cam = Camera.main.transform;
        }

        void Update()
        {
            if (rotating)
            {
                globe.transform.Rotate(0, rotSpeed * Time.deltaTime, 0, Space.World);
            }
            if (globeAnimator != null && globeAnimator.isInitialized)
            {
                int stateHash = globeAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
                if (stateHash == invisibleStateHash)
                {
                    globe.SetActive(false);
                }
            }

            if (pushPinMode && Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.name == "Globe")
                    {
                        Transform newpin = Instantiate(pushPin, this.transform);
                        newpin.position = hit.point;
                        pinPositions.Add(newpin.localPosition);
                    }
                }
            }
        }

        void AirTap()
        {
            //stop rotating
            rotating = false;
            //get hit position
            RaycastHit hit;
            Ray ray = new Ray(cam.position, cam.forward);
            if (Physics.Raycast(ray, out hit) && hit.transform.name == "Globe")
            {
                node.transform.position = hit.point;
                //Vector3 camDirection = cam.forward;
                //camDirection.y = 0;
                //camDirection = camDirection.normalized * 1.5f;
                //Vector3 target = cam.position + camDirection;
                Vector3 target = this.transform.position;
                float d = (hit.point - target).magnitude;
                NodeMovement nodeMovementScript = node.GetComponent<NodeMovement>();
                nodeMovementScript.maxSpeed = d;
                nodeMovementScript.moveTarget = target;
            }
            GetComponent<Collider>().enabled = false;
            if (globeAnimator != null && globeAnimator.isInitialized)
            {
                globeAnimator.SetTrigger(fadesOutHash);
                NetworkMessages.Instance.SendAnimationHash(fadesOutHash, NetworkMessages.AnimationTypes.Trigger);
            }
            if (nodeAnimator != null && nodeAnimator.isInitialized)
            {
                nodeAnimator.SetTrigger(appearsHash);
                NetworkMessages.Instance.SendAnimationHash(appearsHash, NetworkMessages.AnimationTypes.Trigger);
            }
            if (pushPinMode)
            {
                string outVectors = "";
                foreach (Vector3 v in pinPositions)
                {
                    outVectors += "new Vector3(" + v.x + ", " + v.y + ", " + v.z + "), ";
                }
                Debug.Log(outVectors);
            }
        }
        
        
    }
}