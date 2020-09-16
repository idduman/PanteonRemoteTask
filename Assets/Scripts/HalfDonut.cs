using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfDonut : MonoBehaviour
{
    public bool rightOriented = true;
    public float speed = 1.5f;
    //[HideInInspector]
    public float phase;

    private Transform stick;
    private float xRadius, speedPositive, speedNegative;
    // Start is called before the first frame update
    void Start()
    {
        stick = transform.GetChild(0).GetComponent<Transform>();
        xRadius = stick.localScale.x * 0.165f;
        phase = stick.localPosition.x;
        speedPositive = Mathf.Abs(speed);
        speedNegative = -speedPositive;
    }

    // Update is called once per frame
    void Update()
    {
        if (stick.localPosition.x > xRadius)
            speed = speedNegative;
        else if (stick.localPosition.x < -xRadius)
            speed = speedPositive;

        stick.localPosition += Vector3.right * xRadius * speed * Time.deltaTime;
        phase = stick.localPosition.x / xRadius;
    }
}
