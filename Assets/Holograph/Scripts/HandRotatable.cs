using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

namespace Holograph
{
    public class HandRotatable : MonoBehaviour,
                                     IFocusable,
                                     IInputHandler,
                                     ISourceStateHandler
    {
        public event Action StartedDragging;
        public event Action StoppedDragging;

        [Tooltip("Game object that will be dragged.")]
        public GameObject RotatedObject;
        private Transform RotatedObjectTransform;
        private MapRotationListener RotatedMapListener;

        [Tooltip("Radius used to model the dragged object as a ball.")]
        public float HostRadius = 1f;

        public bool IsDraggingEnabled = true;

        private Transform cam;
        private bool isDragging;
        private bool isGazed;
        private float objRefDistance;
        private Vector3 handRefDirection;
        private Quaternion objRefRotation;
        private Quaternion draggingRotation;

        private IInputSource currentInputSource = null;
        private uint currentInputSourceId;

        private void Start()
        {
            if (RotatedObject == null)
            {
                RotatedObject = this.gameObject;
            }
            cam = Camera.main.transform;
            RotatedObjectTransform = RotatedObject.transform;
            RotatedMapListener = RotatedObject.GetComponent<MapRotationListener>();
        }

        private void OnDestroy()
        {
            if (isDragging)
            {
                StopDragging();
            }
            if (isGazed)
            {
                OnFocusExit();
            }
        }

        private void Update()
        {
            if (IsDraggingEnabled && isDragging)
            {
                UpdateDragging();
            }
        }

        public void StartDragging()
        {
            if (!IsDraggingEnabled)
            {
                return;
            }
            if (isDragging)
            {
                return;
            }
            InputManager.Instance.PushModalInputHandler(gameObject);

            Vector3 gazeHitPosition = GetHostHitPosition();
            Vector3 handPosition;
            currentInputSource.TryGetPosition(currentInputSourceId, out handPosition);
            Vector3 pivotPosition = GetHandPivotPosition();
            objRefDistance = Vector3.Magnitude(gazeHitPosition - pivotPosition);
            handRefDirection = (handPosition - pivotPosition).normalized;
            objRefRotation = RotatedObjectTransform.rotation;

            StartedDragging.RaiseEvent();
            isDragging = true;
        }

        private Vector3 GetHostHitPosition()
        {
            Vector3 hostRelativeToCam = RotatedObjectTransform.position - cam.position;
            float hitDistance = hostRelativeToCam.magnitude - HostRadius;
            return cam.TransformVector(hostRelativeToCam.normalized * hitDistance);
        }

        private Vector3 GetHandPivotPosition()
        {
            Vector3 pivot = cam.position + new Vector3(0, -0.2f, 0);
            return pivot;
        }

        private void UpdateDragging()
        {
            Vector3 newHandPosition;
            currentInputSource.TryGetPosition(currentInputSourceId, out newHandPosition);
            Vector3 pivotPosition = GetHandPivotPosition();
            Vector3 newHandDirection = (newHandPosition - pivotPosition).normalized;
            Quaternion handRotation = Quaternion.FromToRotation(handRefDirection, newHandDirection);
            Quaternion hostRatation = Quaternion.Lerp(Quaternion.identity, handRotation, objRefDistance / HostRadius);
            draggingRotation = Quaternion.Inverse(hostRatation) * objRefRotation;
            NetworkMessages.Instance.SendMapRotation(draggingRotation);
            RotatedMapListener.targetRotation = draggingRotation;
        }

        public void StopDragging()
        {
            if (!isDragging)
            {
                return;
            }

            InputManager.Instance.PopModalInputHandler();

            isDragging = false;
            currentInputSource = null;
            StoppedDragging.RaiseEvent();
        }

        public void OnFocusEnter()
        {
            if (!IsDraggingEnabled || isGazed)
            {
                return;
            }
            isGazed = true;
        }

        public void OnFocusExit()
        {
            if (!IsDraggingEnabled || !isGazed)
            {
                return;
            }
            isGazed = false;
        }

        public void OnInputUp(InputEventData eventData)
        {
            if (currentInputSource != null &&
                eventData.SourceId == currentInputSourceId)
            {
                StopDragging();
            }
        }

        public void OnInputDown(InputEventData eventData)
        {
            if (isDragging)
            {
                return;
            }
            currentInputSource = eventData.InputSource;
            currentInputSourceId = eventData.SourceId;
            StartDragging();
        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            if (currentInputSource != null && eventData.SourceId == currentInputSourceId)
            {
                StopDragging();
            }
        }
    }
}