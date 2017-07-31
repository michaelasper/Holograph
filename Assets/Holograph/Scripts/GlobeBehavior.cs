using System;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using HoloToolkit.Sharing;

namespace Holograph
{
    public class GlobeBehavior : MonoBehaviour, IInputHandler
    {
        public GameObject globe;
        public MapManager mapManager;
        [Range(15f, 25f)]
        public float globeRotationSpeed = 20f;
        [Tooltip("If true, clicking on globe creates a pushpin, whose coordinates will be printed when globe is airtapped")]
        public bool pushpinMode;

        public bool rotating = true;
        public GameObject firstNode;
        private Animator globeAnimator;
        private Transform pushPin;
        private List<Vector3> pinPositions;
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
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.FirstNodeTransform] = FirstNodeTransform;
            globeAnimator = globe.GetComponent<Animator>();
        }

        void Update()
        {
            if (rotating)
            {
                this.transform.Rotate(0, globeRotationSpeed * Time.deltaTime, 0, Space.World);
            }
            if (globeAnimator != null && globeAnimator.isInitialized)
            {
                int stateHash = globeAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
                if (stateHash == invisibleStateHash)
                {
                    globe.SetActive(false);
                }
            }
            if (pushpinMode && Input.GetMouseButtonDown(0))
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
            mapManager.transform.position = transform.position;
            mapManager.InitMap();
            mapManager.PositionNodes();
            RaycastHit hit;
            Ray ray = new Ray(cam.position, cam.forward);
            if (Physics.Raycast(ray, out hit) && hit.transform.name == "Globe")
            {
                Vector3 target = this.transform.position;
                if (firstNode != null)
                {
                    firstNode.transform.position = hit.point;
                    NodeMovement nodeMovementScript = firstNode.GetComponent<NodeMovement>();
                    NetworkMessages.Instance.SendFirstNodeTransform();
                    nodeMovementScript.moveTo(target);
                }
            }
            GetComponent<Collider>().enabled = false;
            if (globeAnimator != null && globeAnimator.isInitialized)
            {
                globeAnimator.SetTrigger(fadesOutHash);
            }
            //Used in pushpin mode
            if (pushpinMode)
            {
                string outVectors = "";
                foreach (Vector3 v in pinPositions)
                {
                    outVectors += "new Vector3(" + v.x + ", " + v.y + ", " + v.z + "), ";
                }
                Debug.Log(outVectors);
            }
        }

        public void DefaultStoryEntry()
        {
            rotating = false;
            mapManager.transform.position = transform.position;
            mapManager.InitMap();
            mapManager.PositionNodes();
            firstNode.transform.localPosition = Vector3.zero;
            globeAnimator.SetTrigger(fadesOutHash);
        }
        private void FirstNodeTransform(NetworkInMessage msg)
        {
            //long userId = msg.ReadInt64();
            DefaultStoryEntry();
        }
    }
}