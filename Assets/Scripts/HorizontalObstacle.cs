using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalObstacle : MonoBehaviour
{
    public float speed = 1;
    private float xRadius, speedPositive, speedNegative;
    // Start is called before the first frame update
    void Start()
    {
        xRadius = transform.localScale.x * 0.225f;
        speedPositive = Mathf.Abs(speed);
        speedNegative = -speedPositive;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > xRadius)
            speed = speedNegative;
        else if (transform.position.x < -xRadius)
            speed = speedPositive;

        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}
