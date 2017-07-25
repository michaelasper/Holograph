using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverHighlight : MonoBehaviour
{
    [Range(1f, 2f)]
    public float hightlight = 1.5f;
    private bool isGazedAt;
    private Transform cam;
    private Material objectMaterial;
    // Use this for initialization
    void Start()
    {
        cam = Camera.main.transform;
        objectMaterial = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {

        RaycastHit hit;
        Ray ray = new Ray(cam.position, cam.forward);
        if (Physics.Raycast(ray, out hit) && hit.transform == this.transform)
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