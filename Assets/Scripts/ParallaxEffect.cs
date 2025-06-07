using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ParallaxEffecr : MonoBehaviour
{
    public Camera cam;
    public Transform followTarget;

    Vector2 startingPos;

    float startingZ;

    Vector2 camMoveSinceStart => (Vector2) cam.transform.position - startingPos;

    float zDistanceFromTarget => transform.position.z - followTarget.transform.position.z;
    float clippingPlane => (cam.transform.position.z + (zDistanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane));
    float parallaxFactor => math.abs(zDistanceFromTarget) / clippingPlane;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        startingZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPos = startingPos + camMoveSinceStart * parallaxFactor;

        transform.position = new Vector3(newPos.x, newPos.y, startingZ);
    }
}
