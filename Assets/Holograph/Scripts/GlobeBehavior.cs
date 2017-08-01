// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;
    using System.Collections.Generic;

    using HoloToolkit.Sharing;
    using HoloToolkit.Unity.InputModule;

    using UnityEngine;

    public class GlobeBehavior : MonoBehaviour, IInputHandler
    {
        public GameObject firstNode;

        public GameObject globe;

        [Range(15f, 25f)]
        public float globeRotationSpeed = 20f;

        public MapManager mapManager;

        public AudioSource audioSource;
        public AudioClip enterStorySound;

        [Tooltip("If true, clicking on globe creates a pushpin, whose coordinates will be printed when globe is airtapped")]
        public bool pushpinMode;

        public bool rotating = true;

        private Transform cam;

        private int fadesOutHash;

        private Animator globeAnimator;

        private int invisibleStateHash;

        private List<Vector3> pinPositions;

        private Transform pushPin;

        public void DefaultStoryEntry()
        {
            rotating = false;
            mapManager.transform.position = transform.position;
            mapManager.InitMap();
            mapManager.PositionNodes();
            firstNode.transform.localPosition = Vector3.zero;
            globeAnimator.SetTrigger(fadesOutHash);
        }

        void IInputHandler.OnInputDown(InputEventData eventData)
        {
            RaycastHit hit;
            var ray = new Ray(cam.position, cam.forward);
            if (Physics.Raycast(ray, out hit) && hit.transform.name == "Globe")
            {
                var circleWave = GetComponent<GlobeCircleWave>();
                circleWave.initCircleLines(hit.point);
            }
        }

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            AirTap();
        }

        private void AirTap()
        {
            rotating = false;
            audioSource.PlayOneShot(this.enterStorySound);
            mapManager.transform.position = transform.position;
            mapManager.InitMap();
            mapManager.PositionNodes();
            RaycastHit hit;
            var ray = new Ray(cam.position, cam.forward);
            if (Physics.Raycast(ray, out hit) && hit.transform.name == "Globe")
            {
                var target = transform.position;
                if (firstNode != null)
                {
                    firstNode.transform.position = hit.point;
                    var nodeMovementScript = firstNode.GetComponent<NodeMovement>();
                    NetworkMessages.Instance.SendFirstNodeTransform();
                    nodeMovementScript.moveTo(target);
                }
            }

            GetComponent<Collider>().enabled = false;
            if (globeAnimator != null && globeAnimator.isInitialized)
            {
                globeAnimator.SetTrigger(fadesOutHash);
            }

            // Used in pushpin mode
            if (pushpinMode)
            {
                var outVectors = string.Empty;
                foreach (var v in pinPositions)
                {
                    outVectors += "new Vector3(" + v.x + ", " + v.y + ", " + v.z + "), ";
                }

                Debug.Log(outVectors);
            }
        }

        private void FirstNodeTransform(NetworkInMessage msg)
        {
            // long userId = msg.ReadInt64();
            DefaultStoryEntry();
        }

        private void Start()
        {
            pinPositions = new List<Vector3>();
            invisibleStateHash = Animator.StringToHash("Base Layer.Invisible");
            fadesOutHash = Animator.StringToHash("fadesOut");
            cam = Camera.main.transform;
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.FirstNodeTransform] = FirstNodeTransform;
            globeAnimator = globe.GetComponent<Animator>();
        }

        private void Update()
        {
            if (rotating)
            {
                transform.Rotate(0, globeRotationSpeed * Time.deltaTime, 0, Space.World);
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
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.name == "Globe")
                    {
                        var newpin = Instantiate(pushPin, transform);
                        newpin.position = hit.point;
                        pinPositions.Add(newpin.localPosition);
                    }
                }
            }
        }
    }
}