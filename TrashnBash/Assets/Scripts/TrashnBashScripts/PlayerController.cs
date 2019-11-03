using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;

    [SerializeField]
    private float moveSpeed = 100;
    [SerializeField]
    private float turnSpeed = 5f;
    private float jumpSpeed = 5.0f;
    private float gravity = 20.0f;
    public bool isGrounded = true;

    private void Awake()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }

    private void Update()
    {

        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(horizontal, 0, vertical); ;

        if (Input.GetButtonDown("Jump") )
        {
            move.y = jumpSpeed;
            isGrounded = false;
        }

        move.y -= gravity * Time.deltaTime;
        controller.Move(move * Time.deltaTime * moveSpeed);

        var movement = new Vector3(horizontal, 0, vertical);
        if (movement.magnitude > 0)
        {
            Quaternion newDirection = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, newDirection, Time.deltaTime * turnSpeed);
        }
    }


}