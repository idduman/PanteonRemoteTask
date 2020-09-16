using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingStick : MonoBehaviour
{
    public float rotationSpeed;
    public float phase;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
        phase = Mathf.Atan2(transform.forward.x, transform.forward.z) / Mathf.PI * (rotationSpeed>0 ? 1f : -1f);
    }
}
