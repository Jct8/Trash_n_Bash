using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController)),RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Player player;
    private Camera camera;

    [SerializeField]
    private float moveSpeed = 10.0f;
    [SerializeField]
    private float minMoveSpeed = 10.0f;
    [SerializeField]
    private float maxMoveSpeed = 50.0f;
    [SerializeField]
    private float turnSpeed = 5.0f;
    [SerializeField]
    private float acceleration = 5f;
    [SerializeField]
    private float deacceleration = 5f;

    public bool HoldShiftForAcceleration = false;

    private void Awake()
    {
        controller = gameObject.GetComponent<CharacterController>();
        player = gameObject.GetComponent<Player>();
        camera = Camera.main;
    }

    private void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * vertical + right * horizontal;

        if (move.magnitude >0 && moveSpeed < maxMoveSpeed && !HoldShiftForAcceleration)
        {
            moveSpeed += (acceleration * Time.deltaTime);
        }
        else if(move.magnitude > 0 && moveSpeed < maxMoveSpeed 
            && HoldShiftForAcceleration && Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed += (acceleration * Time.deltaTime);
        }
        else if( moveSpeed > minMoveSpeed )
        {
            moveSpeed -= (deacceleration * Time.deltaTime);
        }
        controller.Move(move * Time.deltaTime * moveSpeed);

        if (move.magnitude > 0)
        {
            Quaternion newDirection = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, newDirection, Time.deltaTime * turnSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(player.Attack());
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            player.PoisonAttack();
        }
    }

   
}