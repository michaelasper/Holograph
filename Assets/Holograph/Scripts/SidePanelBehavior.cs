// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

using UnityEngine;

public class SidePanelBehavior : MonoBehaviour
{
    [Tooltip("Distance from the panel")]

    // [Range(.5f, 2f)]
    public float distance = .5f;

    [Tooltip("Position of panel in field of view. Origin is center")]
    public Vector2 position = new Vector2(-.1f, 0f);

    private Transform cam;

    private Vector3 positionInCam;

    private void Start()
    {
        cam = Camera.main.transform;
        positionInCam = (Vector3)position + distance * Vector3.forward;
    }

    private void Update()
    {
        transform.position = cam.TransformPoint(positionInCam);
        transform.rotation = cam.rotation;
    }
}