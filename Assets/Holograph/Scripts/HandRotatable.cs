// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;

    using HoloToolkit.Unity;
    using HoloToolkit.Unity.InputModule;

    using UnityEngine;

    public class HandRotatable : MonoBehaviour, IFocusable, IInputHandler, ISourceStateHandler
    {
        [Tooltip("Radius used to model the dragged object as a ball.")]
        public float HostRadius = 1f;

        public bool IsDraggingEnabled = true;

        [Tooltip("Game object that will be dragged.")]
        public GameObject RotatedObject;

        private Transform cam;

        private IInputSource currentInputSource;

        private uint currentInputSourceId;

        private Quaternion draggingRotation;

        private Vector3 handRefDirection;

        private bool isDragging;

        private bool isGazed;

        private float objRefDistance;

        private Quaternion objRefRotation;

        private MapRotationListener RotatedMapListener;

        private Transform RotatedObjectTransform;

        public event Action StartedDragging;

        public event Action StoppedDragging;

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

        public void OnInputUp(InputEventData eventData)
        {
            if (currentInputSource != null && eventData.SourceId == currentInputSourceId)
            {
                StopDragging();
            }
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

            var gazeHitPosition = GetHostHitPosition();
            Vector3 handPosition;
            currentInputSource.TryGetPosition(currentInputSourceId, out handPosition);
            var pivotPosition = GetHandPivotPosition();
            objRefDistance = Vector3.Magnitude(gazeHitPosition - pivotPosition);
            handRefDirection = (handPosition - pivotPosition).normalized;
            objRefRotation = RotatedObjectTransform.rotation;

            StartedDragging.RaiseEvent();
            isDragging = true;
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

        private Vector3 GetHandPivotPosition()
        {
            var pivot = cam.position + new Vector3(0, -0.2f, 0);
            return pivot;
        }

        private Vector3 GetHostHitPosition()
        {
            var hostRelativeToCam = RotatedObjectTransform.position - cam.position;
            float hitDistance = hostRelativeToCam.magnitude - HostRadius;
            return cam.TransformVector(hostRelativeToCam.normalized * hitDistance);
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

        private void Start()
        {
            if (RotatedObject == null)
            {
                RotatedObject = gameObject;
            }

            cam = Camera.main.transform;
            RotatedObjectTransform = RotatedObject.transform;
            RotatedMapListener = RotatedObject.GetComponent<MapRotationListener>();
        }

        private void Update()
        {
            if (IsDraggingEnabled && isDragging)
            {
                UpdateDragging();
            }
        }

        private void UpdateDragging()
        {
            Vector3 newHandPosition;
            currentInputSource.TryGetPosition(currentInputSourceId, out newHandPosition);
            var pivotPosition = GetHandPivotPosition();
            var newHandDirection = (newHandPosition - pivotPosition).normalized;
            var handRotation = Quaternion.FromToRotation(handRefDirection, newHandDirection);
            var hostRatation = Quaternion.Lerp(Quaternion.identity, handRotation, objRefDistance / HostRadius);
            draggingRotation = Quaternion.Inverse(hostRatation) * objRefRotation;
            NetworkMessages.Instance.SendMapRotation(draggingRotation);
            RotatedMapListener.targetRotation = draggingRotation;
        }
    }
}