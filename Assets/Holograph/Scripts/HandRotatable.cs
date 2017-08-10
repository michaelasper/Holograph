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

    /// <summary>
    /// Rotates the graph
    /// </summary>
    public class HandRotatable : MonoBehaviour, IFocusable, IInputHandler, ISourceStateHandler
    {
        /// <summary>
        /// The radius of the host's range
        /// </summary>
        [Tooltip("Radius used to model the dragged object as a ball.")]
        public float HostRadius = 1f;

        /// <summary>
        /// Whether or not dragging is enabled
        /// </summary>
        public bool IsDraggingEnabled = true;

        /// <summary>
        /// The rotated object.
        /// </summary>
        [Tooltip("Game object that will be dragged.")]
        public GameObject RotatedObject;

        /// <summary>
        /// The main camera's transform.
        /// </summary>
        private Transform cam;

        /// <summary>
        /// The current input source.
        /// </summary>
        private IInputSource currentInputSource;

        /// <summary>
        /// The current input source id.
        /// </summary>
        private uint currentInputSourceId;

        /// <summary>
        /// The target rotation.
        /// </summary>
        private Quaternion targetRotation;

        /// <summary>
        /// The initial hand direction.
        /// </summary>
        private Vector3 initialHandDirection;

        /// <summary>
        /// True if is dragging
        /// </summary>
        private bool isDragging;

        /// <summary>
        /// True if is gazed
        /// </summary>
        private bool isGazed;

        /// <summary>
        /// Initial object distance
        /// </summary>
        private float initialObjectDistance;

        /// <summary>
        /// Initial object rotation
        /// </summary>
        private Quaternion initialObjectRotation;

        /// <summary>
        /// Handler for all rotations
        /// </summary>
        private MapRotationListener mapRotationHandler;

        /// <summary>
        /// The rotated object transform.
        /// </summary>
        private Transform rotatedObjectTransform;

        /// <summary>
        /// Start dragging event
        /// </summary>
        public event Action StartedDraggingEvent;

        /// <summary>
        /// Stop dragging event.
        /// </summary>
        public event Action StoppedDraggingEvent;

        /// <summary>
        /// Activated when started to be gazed on
        /// </summary>
        public void OnFocusEnter()
        {
            if (!this.IsDraggingEnabled || this.isGazed)
            {
                return;
            }

            this.isGazed = true;
        }

        /// <summary>
        /// Activated when gaze has left object
        /// </summary>
        public void OnFocusExit()
        {
            if (!this.IsDraggingEnabled || !this.isGazed)
            {
                return;
            }

            this.isGazed = false;
        }

        /// <summary>
        /// Activated when clicked on
        /// </summary>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        public void OnInputDown(InputEventData eventData)
        {
            if (this.isDragging)
            {
                return;
            }

            this.currentInputSource = eventData.InputSource;
            this.currentInputSourceId = eventData.SourceId;
            this.StartDragging();
        }

        /// <summary>
        /// Activated when clicked on and has been let go
        /// </summary>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        public void OnInputUp(InputEventData eventData)
        {
            if (this.currentInputSource != null && eventData.SourceId == this.currentInputSourceId)
            {
                this.StopDragging();
            }
        }

        /// <summary>
        /// Activated when source is detected
        /// </summary>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        public void OnSourceDetected(SourceStateEventData eventData)
        {
        }

        /// <summary>
        /// Activated when source has been lost
        /// </summary>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        public void OnSourceLost(SourceStateEventData eventData)
        {
            if (this.currentInputSource != null && eventData.SourceId == this.currentInputSourceId)
            {
                this.StopDragging();
            }
        }

        /// <summary>
        /// Starts dragging the object
        /// </summary>
        public void StartDragging()
        {
            if (!this.IsDraggingEnabled || this.isDragging)
            {
                return;
            }

            InputManager.Instance.PushModalInputHandler(this.gameObject);

            var gazeHitPosition = this.GetHostHitPosition();
            Vector3 handPosition;
            this.currentInputSource.TryGetPosition(this.currentInputSourceId, out handPosition);
            var pivotPosition = this.GetHandPivotPosition();
            this.initialObjectDistance = Vector3.Magnitude(gazeHitPosition - pivotPosition);
            this.initialHandDirection = (handPosition - pivotPosition).normalized;
            this.initialObjectRotation = this.rotatedObjectTransform.rotation;

            this.StartedDraggingEvent.RaiseEvent();
            this.isDragging = true;
        }

        /// <summary>
        /// Stops dragging the object
        /// </summary>
        public void StopDragging()
        {
            if (!this.isDragging)
            {
                return;
            }

            InputManager.Instance.PopModalInputHandler();

            this.isDragging = false;
            this.currentInputSource = null;
            this.StoppedDraggingEvent.RaiseEvent();
        }

        /// <summary>
        /// Gets the position of the pivot
        /// </summary>
        /// <returns>
        /// The <see cref="Vector3"/>.
        /// </returns>
        private Vector3 GetHandPivotPosition()
        {
            var pivot = this.cam.position + new Vector3(0, -0.2f, 0);
            return pivot;
        }

        /// <summary>
        /// Gets the position of the host.
        /// </summary>
        /// <returns>
        /// The <see cref="Vector3"/>.
        /// </returns>
        private Vector3 GetHostHitPosition()
        {
            var hostRelativeToCam = this.rotatedObjectTransform.position - this.cam.position;
            float hitDistance = hostRelativeToCam.magnitude - this.HostRadius;
            return this.cam.TransformVector(hostRelativeToCam.normalized * hitDistance);
        }

        /// <summary>
        /// Called when object is destroyed
        /// </summary>
        private void OnDestroy()
        {
            if (this.isDragging)
            {
                this.StopDragging();
            }

            if (this.isGazed)
            {
                this.OnFocusExit();
            }
        }

        /// <summary>
        /// Called when object is instantiated and active
        /// </summary>
        private void Start()
        {
            if (this.RotatedObject == null)
            {
                this.RotatedObject = this.gameObject;
            }

            this.cam = Camera.main.transform;
            this.rotatedObjectTransform = this.RotatedObject.transform;
            this.mapRotationHandler = this.RotatedObject.GetComponent<MapRotationListener>();
        }

        /// <summary>
        /// Called every frame by Unity
        /// </summary>
        private void Update()
        {
            if (this.IsDraggingEnabled && this.isDragging)
            {
                this.UpdateDragging();
            }

        }

        /// <summary>
        /// Updates rotation value to network and the script that actually does the rotation
        /// </summary>
        private void UpdateDragging()
        {
            Vector3 newHandPosition;
            this.currentInputSource.TryGetPosition(this.currentInputSourceId, out newHandPosition);
            var pivotPosition = this.GetHandPivotPosition();
            var newHandDirection = (newHandPosition - pivotPosition).normalized;
            var handRotation = Quaternion.FromToRotation(this.initialHandDirection, newHandDirection);
            var hostRatation = Quaternion.Lerp(Quaternion.identity, handRotation, this.initialObjectDistance / this.HostRadius);
            this.targetRotation = Quaternion.Inverse(hostRatation) * this.initialObjectRotation;
            NetworkMessages.Instance.SendMapRotation(this.targetRotation);
            this.mapRotationHandler.targetRotation = this.targetRotation;
        }

    }

}