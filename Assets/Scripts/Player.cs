using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [HideInInspector]

    public enum PlayerState {Platform, RotatingPlatform, Finished, Dead};
    public PlayerState playerState;

    public float moveSpeed = 3;
    public Transform sceneCamera;

    private int stunCounter = 0;
    private Transform parent;
    private float maxRotationSpeed,finishPosZ;
    private float mouseMinTreshold,mouseMaxTreshold;
    private Vector3 pivot;
    private Vector3 cameraPos;
    private Vector3 mousePos;
    private Quaternion cameraRot;
    private Animator animator;
    void Start()
    {
        stunCounter = 0;
        maxRotationSpeed = 30f;
        mouseMinTreshold = 0.05f;
        mouseMaxTreshold = 1.5f;
        parent = transform.parent;
        //sceneCamera = transform.GetChild(3);
        cameraPos = sceneCamera.localPosition - transform.localPosition;
        cameraRot = sceneCamera.localRotation;
        finishPosZ = GameObject.Find("Wall").transform.position.z - 4f;
        animator = GetComponent<Animator>();
        animator.SetBool("Running", false);
    }

    void FixedUpdate()
    {
        if (stunCounter > 0)
        {
            MoveCamera();
            stunCounter--;
            return;
        }
        //GetComponent<Animator>().SetTrigger("run");
        switch (playerState)
        {
            case PlayerState.Platform:
                if (Input.GetMouseButtonDown(0))
                {
                    animator.SetBool("Running", true);
                    mousePos = Input.mousePosition;
                }
                if (Input.GetMouseButton(0))
                {
                    animator.SetBool("Running", true);
                    Vector3 inputVector = (Input.mousePosition - mousePos);
                    if (inputVector.magnitude > mouseMinTreshold)
                        RotateTo(RotateInputVector(inputVector));
                    if (inputVector.magnitude > mouseMaxTreshold)
                        mousePos += inputVector * Time.deltaTime;
                    MoveForward();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    animator.SetBool("Running", false);
                }
                MoveCamera();
                break;
            case PlayerState.RotatingPlatform:
                if (Input.GetMouseButtonDown(0))
                {
                    mousePos = Input.mousePosition;
                }
                else if (Input.GetMouseButton(0))
                {
                    Vector3 inputVector = (Input.mousePosition - mousePos);
                    if (inputVector.magnitude > mouseMinTreshold)
                        MoveInRotatingPlaform(inputVector);
                }
                MoveCamera();
                break;
            case PlayerState.Finished:
                FinishMove();
                if (Input.GetMouseButtonDown(0))
                {
                    animator.SetBool("Painting", true);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    animator.SetBool("Painting", false);
                }
                    break;
            case PlayerState.Dead:
                break;
        }
    }
    private Vector3 RotateInputVector(Vector3 inputVector)
    {
        return new Vector3(inputVector.x, 0f, inputVector.y);
    }
    public void Stun(int duration)
    {
        stunCounter = duration;
    }

    private void RotateTo(Vector3 direction)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), maxRotationSpeed * Time.deltaTime);
    }

    private void MoveForward()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

	private void MoveInRotatingPlaform(Vector3 direction)
	{
        float rotSpeedMultiplier = -60;
        float xComp = direction.normalized.x;
        float yComp = direction.normalized.y;
        transform.RotateAround(pivot, Vector3.forward, xComp * rotSpeedMultiplier * Time.deltaTime);
        //Vector3 lookDir = Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector3.forward) * RotateInputVector(direction);
        transform.position += Vector3.forward * yComp * moveSpeed * Time.deltaTime;
    }

    private void FinishMove()
    {
        transform.position = Vector3.Lerp(transform.position,
            new Vector3(0f, transform.position.y, finishPosZ),
            moveSpeed/2 * Time.deltaTime);

        sceneCamera.position = Vector3.Lerp(sceneCamera.position, new Vector3(0f, 2.15f, 120.5f), 3 * Time.deltaTime);
        sceneCamera.rotation = Quaternion.Lerp(sceneCamera.rotation, Quaternion.identity, 3 * Time.deltaTime);
    }

    public void Reset()
    {
        sceneCamera.localPosition = cameraPos;
        sceneCamera.localRotation = cameraRot;
        playerState = PlayerState.Platform;

    }

    public void MoveCamera()
    {
        sceneCamera.localPosition = transform.localPosition + cameraPos;
        sceneCamera.localRotation = cameraRot;
    }
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Obstacle" || col.gameObject.tag == "Horizontal")
        {
            playerState = PlayerState.Dead;
        }
    }

	private void OnTriggerEnter(Collider other)
	{
        if (other.gameObject.tag == "RotatingPlatform_trigger")
        {
            playerState = PlayerState.RotatingPlatform;
        }
        else if (other.gameObject.tag == "Finish")
        {
            animator.SetTrigger("Finished");
            playerState = PlayerState.Finished;
        }
    }

	private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "RotatingPlatform")
        {
            pivot = other.transform.position;
            float rotSpeed = other.GetComponent<RotatingPlatform>().rotationSpeed;
            transform.RotateAround(pivot, Vector3.forward, rotSpeed*Time.deltaTime);
            sceneCamera.localRotation = cameraRot;
        }
    }

	private void OnTriggerExit(Collider other)
	{
        if (other.gameObject.tag == "RotatingPlatform_trigger")
        {
            playerState = PlayerState.Platform;
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }
    }
}
