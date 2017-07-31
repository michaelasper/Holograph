// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

using UnityEngine;

public class HoverHighlight : MonoBehaviour
{
    [Range(1f, 2f)]
    public float hightlight = 1.5f;

    private Transform cam;

    private bool isGazedAt;

    private Material objectMaterial;

    // Use this for initialization
    private void Start()
    {
        cam = Camera.main.transform;
        objectMaterial = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    private void Update()
    {
        RaycastHit hit;
        var ray = new Ray(cam.position, cam.forward);
        if (Physics.Raycast(ray, out hit) && hit.transform == transform)
        {
            if (!isGazedAt)
            {
                isGazedAt = true;
                objectMaterial.color *= hightlight;
            }
        }
        else if (isGazedAt)
        {
            isGazedAt = false;
            objectMaterial.color /= hightlight;
        }
    }
}