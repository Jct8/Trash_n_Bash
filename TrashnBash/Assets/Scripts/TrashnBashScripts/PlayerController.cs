using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController)), RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    private CharacterController _controller;
    private Player _player;
    private Camera _mainCamera;
    private GameObject _lockedOnEnemyGO = null;
    private GameObject _Barricade = null;
    private GameObject _RepairBarricade = null;

    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float minMoveSpeed = 10.0f;
    [SerializeField] private float maxMoveSpeed = 50.0f;
    [SerializeField] private float turnSpeed = 5.0f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deacceleration = 5f;
    [SerializeField] private float gravity = 1.0f;
    [SerializeField] private float burstSpeed = 40.0f;

    [SerializeField] private KeyCode _AttackButton = KeyCode.Space;
    [SerializeField] private KeyCode _PoisonAttackButton = KeyCode.E;
    [SerializeField] private KeyCode _LockTargetButton = KeyCode.Mouse0;
    [SerializeField] private KeyCode _UltimateButton = KeyCode.Q;
    [SerializeField] private KeyCode _PickUpButton = KeyCode.F;
    [SerializeField] private KeyCode _RepairButton = KeyCode.R;
    [SerializeField] private KeyCode _ReleaseLockButton = KeyCode.LeftShift;

    private bool _isTargetLockedOn = false;
    private bool _isHoldingItem = false;
    private bool _isRepairing = false;
    private bool _CanMove = true;

    #region Unity Functions

    private void Awake()
    {
        _controller = gameObject.GetComponent<CharacterController>();
        _player = gameObject.GetComponent<Player>();
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        CalculateMovement();

        if (_isRepairing)
        {
            _isRepairing = _RepairBarricade.GetComponent<Barricade>().CheckRepairValid(transform);
        }

        if (Input.GetKeyDown(_PickUpButton))
        {
            if (!_isHoldingItem)
            {
                _Barricade = _player.DetectBarricade();
                if (_Barricade == null)
                    _isHoldingItem = false;
                else if (_Barricade.GetComponent<Barricade>().CanBePickedUp())
                {
                    _Barricade.GetComponent<Barricade>().PickUp(gameObject);
                    _isHoldingItem = true;
                }
            }
            else
            {
                StartCoroutine(PlaceBarricade());
            }

        }

        if (_isHoldingItem)
            return;

        if (Input.GetKeyDown(_RepairButton))
        {
            _RepairBarricade = _player.DetectBarricade();
            if (_RepairBarricade == null)
                _isHoldingItem = false;
            else
            {
                _isRepairing = true;
                _RepairBarricade.GetComponent<Barricade>().inRangeRepair  = true;
                StartCoroutine(_RepairBarricade.GetComponent<Barricade>().Repair());
            }
        }

        if (Input.GetKeyDown(_AttackButton))
        {
            StartCoroutine(_player.Attack());
        }

        if (Input.GetKeyDown(_PoisonAttackButton))
        {
            StartCoroutine(_player.PoisonAttack());
        }

        if (Input.GetKeyDown(_LockTargetButton))
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

        if (Input.GetKeyDown(_UltimateButton))
        {
            _player.UltimateAttack();
        }

        if (Input.GetKeyDown(_ReleaseLockButton))
        {
            if (_isTargetLockedOn)
            {
                //Deselect
                _isTargetLockedOn = false;
                _lockedOnEnemyGO.GetComponent<Enemy>().SwitchOnTargetIndicator(false);
                _lockedOnEnemyGO = null;
            }
        }
        ActivateTargetLockedOn();

    }

    #endregion

    #region Actions

    public void CalculateMovement()
    {
        if (!_CanMove)
            return;

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

    public IEnumerator PlaceBarricade()
    {
        _CanMove = false;
        yield return new WaitForSeconds(_Barricade.GetComponent<Barricade>()._barricadeBuildTime);
        _Barricade.GetComponent<Barricade>().PlaceBarricade();
        _Barricade = null;
        _isHoldingItem = false;
        _CanMove = true;
    }

    #endregion

    #region Target Lock on

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

    public void SwitchAutoLock(GameObject enemy)
    {
        _lockedOnEnemyGO?.GetComponent<Enemy>().SwitchOnTargetIndicator(false);

        _lockedOnEnemyGO = enemy;
        _isTargetLockedOn = true;
        _lockedOnEnemyGO?.GetComponent<Enemy>().SwitchOnTargetIndicator(true);
    }

    #endregion

    #region Getters

    public GameObject GetLockedOnTarget()
    {
        return _lockedOnEnemyGO;
    }

    public CharacterController GetController()
    {
        return _controller;
    }

    #endregion

}