using System;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Holograph
{
    public class GlobeBehavior : MonoBehaviour, IInputHandler
    {

        public GameObject globe;
        public GameObject firstNode;

        public float rotSpeed = 20f;

        public Animator globeAnimator;
        public MapManager mapManager;
        public bool pushPinMode;
        public Transform pushPin;
        public bool rotating;

        public List<Vector3> pinPositions;

        private int invisibleStateHash;
        private int fadesOutHash;
        private Transform cam;

        void IInputHandler.OnInputDown(InputEventData eventData)
        {
            RaycastHit hit;
            Ray ray = new Ray(cam.position, cam.forward);
            if (Physics.Raycast(ray, out hit) && hit.transform.name == "Globe")
            {
                GlobeCircleWave circleWave = GetComponent<GlobeCircleWave>();
                circleWave.initCircleLines(hit.point);
            }
        }

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            AirTap();
        }

        void Start()
        {
            pinPositions = new List<Vector3>();
            invisibleStateHash = Animator.StringToHash("Base Layer.Invisible");
           
            fadesOutHash = Animator.StringToHash("fadesOut");
            cam = Camera.main.transform;
        }

        void Update()
        {
            if (rotating)
            {
                this.transform.Rotate(0, rotSpeed * Time.deltaTime, 0, Space.World);
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
            rotating = false;

            mapManager.initMap();
            mapManager.positionNodes();

            RaycastHit hit;
            Ray ray = new Ray(cam.position, cam.forward);
            if (Physics.Raycast(ray, out hit) && hit.transform.name == "Globe")
            {
                Vector3 target = this.transform.position;
                if (firstNode != null)
                {
                    firstNode.transform.position = hit.point;
                    NodeMovement nodeMovementScript = firstNode.GetComponent<NodeMovement>();
                    nodeMovementScript.moveTo(target);
                }
            }
            GetComponent<Collider>().enabled = false;
            if (globeAnimator != null && globeAnimator.isInitialized)
            {
                globeAnimator.SetTrigger(fadesOutHash);
                NetworkMessages.Instance.SendAnimationHash(fadesOutHash, NetworkMessages.AnimationTypes.Trigger);
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