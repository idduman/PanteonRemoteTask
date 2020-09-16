using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    public float rotationSpeed;
    void Start()
    {

    }

    void Update()
    {
        transform.RotateAround(transform.position, Vector3.forward , rotationSpeed * Time.deltaTime);
    }
}
