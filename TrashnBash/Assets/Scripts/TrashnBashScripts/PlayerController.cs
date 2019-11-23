using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController)),RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    private CharacterController _controller;
    private Player _player;
    private Camera _mainCamera;
    private GameObject _lockedOnEnemyGO = null;

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

    private void Awake()
    {
        _controller = gameObject.GetComponent<CharacterController>();
        _player = gameObject.GetComponent<Player>();
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(_player.Attack());
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            _player.PoisonAttack();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (_isTargetLockedOn)
            {
                GameObject prevTarget = _lockedOnEnemyGO;
                CheckTargetLockedOn();

                if (prevTarget == _lockedOnEnemyGO)
                {
                    //Deselect
                    _isTargetLockedOn = false;
                    _lockedOnEnemyGO.GetComponent<Enemy>().SwitchOnTargetIndicator(false);
                    _lockedOnEnemyGO = null;
                }
                else
                {
                    _lockedOnEnemyGO = prevTarget;
                }
            }
            else
            {
                CheckTargetLockedOn();
                if (_lockedOnEnemyGO)
                {
                    //Select
                    _isTargetLockedOn = true;
                    _lockedOnEnemyGO.GetComponent<Enemy>().SwitchOnTargetIndicator(true);
                }
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

        Vector3 forward = _mainCamera.transform.forward;
        Vector3 right = _mainCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * vertical + right * horizontal;

        if (move.magnitude == 0.0f)
        {
            moveSpeed = minMoveSpeed + burstSpeed;
        }

        if (move.magnitude > 0 && moveSpeed < maxMoveSpeed)
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

        _controller.Move(move * Time.deltaTime * moveSpeed);
    }

    public void ActivateTargetLockedOn()
    {
        if (_isTargetLockedOn && _lockedOnEnemyGO)
        {
            if (_lockedOnEnemyGO.activeInHierarchy)
            {
                Vector3 targetDirection = _lockedOnEnemyGO.transform.position;
                targetDirection.y = transform.position.y;
                transform.LookAt(targetDirection);
            }
            else
            {
                //Deselect
                _lockedOnEnemyGO.GetComponent<Enemy>().SwitchOnTargetIndicator(false);
                _isTargetLockedOn = false;
                _lockedOnEnemyGO = null;
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
                _lockedOnEnemyGO = hit.transform.gameObject;
            else
                _lockedOnEnemyGO = null;
        }
    }
    
}