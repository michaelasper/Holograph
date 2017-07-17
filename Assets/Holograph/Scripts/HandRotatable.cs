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
        /// <summary>
        /// Event triggered when dragging starts.
        /// </summary>
        public event Action StartedDragging;

        /// <summary>
        /// Event triggered when dragging stops.
        /// </summary>
        public event Action StoppedDragging;

        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        public Transform HostTransform;

        [Tooltip("Radius used to model the dragged object as a ball.")]
        public float HostRadius = 1f;

        [Tooltip("Controls the speed at which the object will interpolate toward the desired rotation")]
        [Range(0.01f, 1.0f)]
        public float RotationLerpSpeed = 0.2f;

        public bool IsDraggingEnabled = true;

        private Transform cam;
        public bool isDragging;
        private bool isGazed;
        private float objRefDistance;
        private Vector3 handRefDirection;
        private Quaternion objRefRotation;

        private float objRedRotationEulerY;

        private Quaternion draggingRotation;

        private IInputSource currentInputSource = null;
        private uint currentInputSourceId;

        private void Start()
        {
            if (HostTransform == null)
            {
                HostTransform = transform;
            }

            cam = Camera.main.transform;
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

        /// <summary>
        /// Starts dragging the object.
        /// </summary>
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

            // Add self as a modal input handler, to get all inputs during the manipulation
            InputManager.Instance.PushModalInputHandler(gameObject);

            Vector3 gazeHitPosition = GetHostHitPosition();
            Vector3 handPosition;
            currentInputSource.TryGetPosition(currentInputSourceId, out handPosition);

            Vector3 pivotPosition = GetHandPivotPosition();
            objRefDistance = Vector3.Magnitude(gazeHitPosition - pivotPosition);

            handRefDirection = (handPosition - pivotPosition).normalized;

            objRefRotation = HostTransform.rotation;
            objRedRotationEulerY = HostTransform.rotation.eulerAngles.y;


            objRefRotation = HostTransform.rotation;




            StartedDragging.RaiseEvent();
            isDragging = true;
        }

        private Vector3 GetHostHitPosition()
        {
            Vector3 hostRelativeToCam = HostTransform.position - cam.position;
            float hitDistance = hostRelativeToCam.magnitude - HostRadius;
            return cam.TransformVector(hostRelativeToCam.normalized * hitDistance);
        }

        /// <summary>
        /// Gets the pivot position for the hand, which is approximated to the base of the neck.
        /// </summary>
        /// <returns>Pivot position for the hand.</returns>
        private Vector3 GetHandPivotPosition()
        {
            Vector3 pivot = cam.position + new Vector3(0, -0.2f, 0); // a bit lower
            return pivot;
        }

        /// <summary>
        /// Update the position of the object being dragged.
        /// </summary>
        private void UpdateDragging()
        {
            Vector3 newHandPosition;
            currentInputSource.TryGetPosition(currentInputSourceId, out newHandPosition);

            Vector3 pivotPosition = GetHandPivotPosition();

            Vector3 newHandDirection = (newHandPosition - pivotPosition).normalized;


            Quaternion handRotation = Quaternion.FromToRotation(handRefDirection, newHandDirection); // Vector3.Angle(newHandDirection, handRefDirection) * Mathf.Sign(Vector3.Cross(newHandDirection, handRefDirection).y);
            
            // Scale the rotation
            Quaternion hostRatation = Quaternion.Lerp(Quaternion.identity, handRotation, objRefDistance / HostRadius);
            draggingRotation = objRefRotation * Quaternion.Inverse(hostRatation); //Quaternion.Euler(0f, objRedRotationEulerY + hostRatationAngle, 0f);


            float handRotatedAngle = Vector3.Angle(newHandDirection, handRefDirection) * Mathf.Sign(Vector3.Cross(newHandDirection, handRefDirection).y);

            float hostRatationAngle = handRotatedAngle * objRefDistance / HostRadius;
            draggingRotation = Quaternion.Euler(0f, objRedRotationEulerY + hostRatationAngle, 0f);

            HostTransform.rotation = Quaternion.Lerp(HostTransform.rotation, draggingRotation, RotationLerpSpeed);

        }

        /// <summary>
        /// Stops dragging the object.
        /// </summary>
        public void StopDragging()
        {
            if (!isDragging)
            {
                return;
            }

            // Remove self as a modal input handler
            InputManager.Instance.PopModalInputHandler();

            isDragging = false;
            currentInputSource = null;
            StoppedDragging.RaiseEvent();
        }

        public void OnFocusEnter()
        {
            if (!IsDraggingEnabled)
            {
                return;
            }

            if (isGazed)
            {
                return;
            }

            isGazed = true;
        }

        public void OnFocusExit()
        {
            if (!IsDraggingEnabled)
            {
                return;
            }

            if (!isGazed)
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
                // We're already handling drag input, so we can't start a new drag operation.
                return;
            }


            currentInputSource = eventData.InputSource;
            currentInputSourceId = eventData.SourceId;
            StartDragging();
        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            // Nothing to do
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