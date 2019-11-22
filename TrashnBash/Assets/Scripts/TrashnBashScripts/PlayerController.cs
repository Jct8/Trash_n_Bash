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
    [SerializeField]
    private float gravity = 1.0f;
    [SerializeField]
    private float burstSpeed = 40.0f;

    private bool _isTargetLockedOn = false;
    private GameObject lockedOnEnemyGO = null;

    public bool HoldShiftForAcceleration = false;

    private void Awake()
    {
        controller = gameObject.GetComponent<CharacterController>();
        player = gameObject.GetComponent<Player>();
        camera = Camera.main;
    }

    private void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(player.Attack());
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            player.PoisonAttack();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (_isTargetLockedOn)
            {
                GameObject prevTarget = lockedOnEnemyGO;
                CheckTargetLockedOn();

                if (prevTarget == lockedOnEnemyGO)
                {
                    _isTargetLockedOn = false;
                    lockedOnEnemyGO = null;
                }
                else
                {
                    lockedOnEnemyGO = prevTarget;
                }
            }
            else
            {
                CheckTargetLockedOn();
                if (lockedOnEnemyGO)
                    _isTargetLockedOn = true;
                else
                    _isTargetLockedOn = false;
            }
        }

        ActivateTargetLockedOn();

    }

    public void CalculateMovement()
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

        if (move.magnitude == 0.0f)
        {
            moveSpeed = minMoveSpeed + burstSpeed;
        }

        if (move.magnitude > 0 && moveSpeed < maxMoveSpeed && !HoldShiftForAcceleration)
        {
            moveSpeed += (acceleration * Time.deltaTime);
        }
        else if (move.magnitude > 0 && moveSpeed < maxMoveSpeed
            && HoldShiftForAcceleration && Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed += (acceleration * Time.deltaTime);
        }
        else if (moveSpeed > minMoveSpeed)
        {
            moveSpeed -= (deacceleration * Time.deltaTime);
        }

        if (move.magnitude > 0)
        {
            Quaternion newDirection = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, newDirection, Time.deltaTime * turnSpeed);
        }

        move.y -= gravity;

        controller.Move(move * Time.deltaTime * moveSpeed);
    }

    public void ActivateTargetLockedOn()
    {
        if (_isTargetLockedOn && lockedOnEnemyGO)
        {
            if (lockedOnEnemyGO.activeInHierarchy)
            {
                Vector3 targetDirection = lockedOnEnemyGO.transform.position;
                targetDirection.y = transform.position.y;
                transform.LookAt(targetDirection);
            }
            else
            {
                _isTargetLockedOn = false;
                lockedOnEnemyGO = null;
            }
        }
    }

    public void CheckTargetLockedOn()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.CompareTag("Enemy"))
                lockedOnEnemyGO = hit.transform.gameObject;
            else
                lockedOnEnemyGO = null;
        }
    }
    
}