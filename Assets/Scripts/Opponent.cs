using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    public float moveSpeed = 3;
    public float maxRotation = 60;
    public float turnSpeed = 20;
    [HideInInspector]
    public bool isFinished;

    private enum ControlState {Default, Platform, Obstacle, Donut, Horizontal, RotatingPlatform, RotatingStick, Finished};
    private ControlState controlState;

    private int randNumber,stunCounter;
    private Vector3 pivot, initPos;
    private float xPos,zPos;
    private float donutPhase, donutTriggerPosZ, rotatingPlatformForwardComp, stickPhase, stickTriggerPosZ, stickRotSpeed;
    private bool donutRightOriented;
    private Animator animator;

    void Start()
    {
        stunCounter = 0;
        isFinished = false;
        initPos = transform.position; // set the initial position
        xPos = transform.position.x; // Initialize x posiiton variable
        zPos = transform.position.z; // Initialize z posiiton variable
        controlState = ControlState.Default; // Default control state
        donutPhase = donutTriggerPosZ = stickPhase = stickTriggerPosZ = 0f; // Initialize the obstacle readings
        rotatingPlatformForwardComp = 1f;
        donutRightOriented = true; // orientation of the current donut obstacle
        randNumber = 0; // default movement randomizer number
        animator = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        xPos = transform.position.x;
        zPos = transform.position.z;

        if (stunCounter > 0)
        {
            stunCounter--;
            return;
        }

        switch (controlState) //state control flow
        {
            case ControlState.Default:
                MoveDefault();
                break;
            case ControlState.Platform:
                MovePlatform();
                break;
            case ControlState.Obstacle:
                AvoidObstacle();
                break;
            case ControlState.Donut:
                AvoidDonut();
                break;
            case ControlState.Horizontal:
                AvoidHorizontal();
                break;
            case ControlState.RotatingStick:
                AvoidRotatingStick();
                break;
            case ControlState.RotatingPlatform:
                MoveRotatingPlatform();
                break;
            case ControlState.Finished:
                break;
        }
    }
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Obstacle" || col.gameObject.tag == "Horizontal")
        {
            transform.position = initPos;
            transform.rotation = Quaternion.identity;
        }
    }
	private void OnTriggerEnter(Collider other)
	{
        switch (other.gameObject.tag)
        {
            case "Platform":
                controlState = ControlState.Platform;
                break;
            case "Obstacle_trigger":
                controlState = ControlState.Obstacle;
                break;
            case "Donut_trigger":
                controlState = ControlState.Donut;
                donutPhase = other.GetComponent<HalfDonut>().phase;
                donutRightOriented = other.GetComponent<HalfDonut>().rightOriented;
                donutTriggerPosZ = other.bounds.center.z;
                break;
            case "Horizontal_trigger":
                controlState = ControlState.Horizontal;
                break;
            case "RotatingStick":
                controlState = ControlState.RotatingStick;
                break;
            case "RotatingPlatform_trigger":
                controlState = ControlState.RotatingPlatform;
                break;
            case "Finish":
                isFinished = true;
                animator.SetTrigger("Victory");
                GetComponent<Rigidbody>().isKinematic = true;
                GetComponent<Collider>().enabled = false;
                controlState = ControlState.Finished;
                break;
        }
	}
	private void OnTriggerExit(Collider other)
	{
        controlState = ControlState.Default;
	}
	private void OnTriggerStay(Collider other)
	{
        if (other.gameObject.tag == "RotatingPlatform")
        {
            float rotSpeed = other.GetComponent<RotatingPlatform>().rotationSpeed;
            pivot = other.transform.position;
            if (other.bounds.center.z > transform.position.z)
            {
                transform.RotateAround(pivot, Vector3.forward, rotSpeed / 2f * Time.deltaTime);
                rotatingPlatformForwardComp = 0.35f;
            }
            else if (xPos < -0.1f || xPos > 0.1f)
            {
                transform.RotateAround(pivot, Vector3.forward, -rotSpeed * Time.deltaTime);
                rotatingPlatformForwardComp = 0.5f;
            }
            else
            {
                rotatingPlatformForwardComp = 0.5f;
            }
        }
        else if (other.gameObject.tag == "Donut_trigger")
        {
            donutPhase = other.GetComponent<HalfDonut>().phase;
        }
        else if (other.gameObject.tag == "RotatingStick_trigger")
        {
            stickTriggerPosZ = other.transform.position.z;
            stickRotSpeed = other.GetComponent<RotatingStick>().rotationSpeed;
            stickPhase = other.GetComponent<RotatingStick>().phase;
        }
    }
    public void Stun(int duration)
    {
        stunCounter = duration;
    }
	private void Stop()
	{
        animator.SetBool("Running", false);
	}
	private void MoveForward()
	{
        animator.SetBool("Running", true);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
    private void TurnStraight()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, turnSpeed * Time.deltaTime);
    }
    private void TurnRight()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, maxRotation, 0f), turnSpeed * Time.deltaTime);
    }
    private void TurnLeft()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, -maxRotation, 0f), turnSpeed * Time.deltaTime);
    }
    private void MoveDefault()
    {
        int bound = 60;
        float xTreshold = 2f;
        if (randNumber > bound)
        {
            if(xPos > -xTreshold)
                TurnLeft();
            else
                TurnStraight();
            randNumber--;
        }
        else if (randNumber > 0)
        {
            TurnStraight();
            randNumber--;
        }
        else if (randNumber < -bound)
        {
            if (xPos < xTreshold)
                TurnRight();
            else
                TurnStraight();
            randNumber++;
        }
        else if (randNumber < 0)
        {
            TurnStraight();
            randNumber++;
        }
        else
        {
            randNumber = Random.Range(-100, 100);
        }
        MoveForward();
    }
    private void MovePlatform()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f), 5 * Time.deltaTime);
        if (xPos > 0.1f)
            TurnLeft();
        else if (xPos < -0.1f)
            TurnRight();
        else
            TurnStraight();
        MoveForward();
    }
    private void MoveRotatingPlatform()
    {
        transform.position += transform.forward * rotatingPlatformForwardComp * moveSpeed * Time.deltaTime;
    }
    private void AvoidObstacle()
    {
        float visionTreshold = 1.3f;
        int obstacleState = 0;
        Vector3 leftPos = transform.position + new Vector3(-0.15f, 0.2f, 0f);
        Vector3 rightPos = transform.position + new Vector3(0.15f, 0.2f, 0f);
        RaycastHit hitLeftArm, hitRightArm;
        Ray rayLeftArm = new Ray(leftPos, Vector3.forward);
        Ray rayRightArm = new Ray(rightPos, Vector3.forward);
        if (Physics.Raycast(rayLeftArm, out hitLeftArm, visionTreshold))
        {
            //Debug.DrawLine(leftPos, hitLeftArm.point, Color.red);
            obstacleState += 1;
        }
        if (Physics.Raycast(rayRightArm, out hitRightArm, visionTreshold))
        {
            //Debug.DrawRay(rightPos, hitRightArm.point, Color.green);
            obstacleState += 2;
        }

        if (transform.position.x < -2.3f) //Override if near edge
            obstacleState = 1;
        else if (transform.position.x > 2.3f)
            obstacleState = 2;

        //Debug.Log(obstacleState);
        switch (obstacleState)
        {
            case 0: // if the front is clear, turn straight again
                TurnStraight();
                break;
            case 1: // only left side detects the obstacle, turn right
                TurnRight();
                break;
            case 2: // only right side detects the obstacle, turn left
                TurnLeft();
                break;
            case 3: // both sides detect the obstacle, then check self position relative to the center of osbtacle
                if (hitRightArm.collider.transform.localPosition.x < transform.localPosition.x)
                    TurnRight();
                else
                    TurnLeft();
                break;
        }

        MoveForward();
    }
    private void AvoidHorizontal()
    {
        int obstacleState = 0;
        float visionTreshold = 1.2f;
        float xTreshold = 2.25f;
        Vector3 leftPos = transform.position + new Vector3(-0.2f, 0.2f, 0f);
        Vector3 rightPos = transform.position + new Vector3(0.2f, 0.2f, 0f);
        RaycastHit hitLeftArm, hitRightArm;
        Ray rayLeftArm = new Ray(leftPos, Vector3.forward);
        Ray rayRightArm = new Ray(rightPos, Vector3.forward);
        if (Physics.Raycast(rayLeftArm, out hitLeftArm, visionTreshold) && hitLeftArm.collider.tag == "Horizontal")
        {
            //Debug.DrawLine(leftPos, hitLeftArm.point, Color.red);
            obstacleState += 1;
        }
        if (Physics.Raycast(rayRightArm, out hitRightArm, visionTreshold) && hitRightArm.collider.tag == "Horizontal")
        {
            //Debug.DrawLine(leftPos, hitLeftArm.point, Color.red);
            obstacleState += 2;
        }

        switch (obstacleState)
        {
            case 0: // if the front is clear, turn straight again
                TurnStraight();
                MoveForward();
                break;
            case 1: // only left side detects the obstacle, turn right
                if (hitLeftArm.collider.GetComponent<HorizontalObstacle>().speed < 0 && xPos < xTreshold)
                {
                    TurnRight();
                    MoveForward();
                }
                else
                    Stop();
                break;
            case 2: // only right side detects the obstacle, turn left
                if (hitRightArm.collider.GetComponent<HorizontalObstacle>().speed > 0 && xPos > -xTreshold)
                {
                    TurnLeft();
                    MoveForward();
                }
                else
                    Stop();
                break;
            case 3: // both sides detect the obstacle, then check self position relative to the center of osbtacle
                break;
        }
    }
    private void AvoidDonut()
    {
        float xTreshold = 1.6f;
        float phaseTreshold = 0.15f;
		if (donutRightOriented)
		{
            if (xPos < xTreshold)
            {
                TurnRight();
                MoveForward();
            }
            else if (zPos < donutTriggerPosZ || donutPhase > phaseTreshold)
            {
                TurnStraight();
                MoveForward();
            }
            else Stop();
        }
        else
        {
            if (xPos > -xTreshold)
            {
                TurnLeft();
                MoveForward();
            }
            else if (zPos < donutTriggerPosZ || donutPhase > phaseTreshold)
            {
                TurnStraight();
                MoveForward();
            }
            else Stop();
        }
    }
    private void AvoidRotatingStick()
    {
        float xTresholdCenter = 0.7f;
        float xTresholdEdge = 2.25f;
        if (zPos < stickTriggerPosZ)
        {
            Debug.DrawLine(transform.position, transform.up, Color.red);
            if (Mathf.Abs(xPos) < xTresholdCenter) // Center
            {
                if (stickPhase > -0.85f && stickPhase < 0.85f)
                {
                    if (stickRotSpeed < 0)
                        TurnLeft();
                    else
                        TurnRight();
                    MoveForward();
                }
            }
            else if (xPos < xTresholdCenter) // Left Side
            {
                if (stickRotSpeed < 0)
                {
                    if (stickPhase < -0.3f)
                        TurnStraight();
                    else
                    {
                        if (xPos > -xTresholdEdge)
                            TurnLeft();
                        else
                            TurnStraight();
                    }
                }
                else
                    TurnStraight();
                MoveForward();
            }
            else if (xPos > xTresholdCenter)
            {
                if (stickRotSpeed < 0)
                {
                    if (stickPhase < -0.3f)
                        TurnStraight();
                    else
                    {
                        if (xPos < xTresholdEdge)
                            TurnRight();
                        else
                            TurnStraight();
                    }

                }
                else
                    TurnStraight();
                MoveForward();
            }
        }
        else
        {
            if (stickRotSpeed < 0)
            {
                if (xPos < xTresholdEdge)
                    TurnRight();
                else
                    TurnStraight();
            }
            else
            {
                if (xPos > -xTresholdEdge)
                    TurnLeft();
                else
                    TurnStraight();
            }
            MoveForward();
        }
    }
}
