using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController)), RequireComponent(typeof(Player)), RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    #region Variables
    private Action OnRecycle;

    private CharacterController _controller;
    private Player _player;
    private Tower _tower = null;
    private Camera _mainCamera;
    private GameObject _lockedOnEnemyGO = null;
    private GameObject _Barricade = null;
    private GameObject _RepairBarricade = null;
    //private GameObject _Resource = null;
    private UIManager uiManager;
    private NavMeshAgent agent;
    public List<GameObject> _Resources;

    [Header("Unit Status")]
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float minMoveSpeed = 10.0f;
    [SerializeField] private float maxMoveSpeed = 50.0f;
    [SerializeField] private float turnSpeed = 5.0f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deacceleration = 5f;
    [SerializeField] private float gravity = 1.0f;
    [SerializeField] private float burstSpeed = 40.0f;
    [SerializeField] private float attackCoolDown = 0.4f;
    [SerializeField] private float poisonAttackCoolDown = 3.0f;
    [SerializeField] private float intimidateAttackCoolDown = 5.0f;


    [Header("Trash Cans")]
    [SerializeField][Tooltip("Timer for digging a trash cans")] private float diggingTime = 2.0f;
    [SerializeField][Tooltip("Limit to get trashes from a trash cans")] private int limitOfHolding = 3;
    private bool _isDigging = false;
    public int currentTrashes = 0;

    [SerializeField] private KeyCode _AttackButton = KeyCode.Space;
    [SerializeField] private KeyCode _PoisonAttackButton = KeyCode.E;
    [SerializeField] private KeyCode _LockTargetButton = KeyCode.Mouse0;
    [SerializeField] private KeyCode _UltimateButton = KeyCode.Q;
    [SerializeField] private KeyCode _PickUpButton = KeyCode.F;
    [SerializeField] private KeyCode _RepairButton = KeyCode.R;
    [SerializeField] private KeyCode _ReleaseLockButton = KeyCode.LeftShift;
    [SerializeField] private KeyCode _Intimidate = KeyCode.LeftControl;
    [SerializeField] private KeyCode _ClickMovementButton = KeyCode.Mouse1;
    [SerializeField] private KeyCode _ClickRestoreButton = KeyCode.H;
    [SerializeField] private KeyCode _ClickTowerButton = KeyCode.Mouse0;

    private bool _isTargetLockedOn = false;
    private bool _isHoldingItem = false;
    private bool _isRepairing = false;
    private bool _CanMove = true;
   
    private float currentAttackCoolDown = 0.0f;
    private float currentPoisonAttackCoolDown = 0.0f;
    private float currentIntimidateAttackCoolDown = 0.0f;
    private float holdClickTime = 0.0f;

    public float allowedRangeofResource = 3.5f;
    public float holdClickTimeMax = 2.0f;
    public bool isUsingMouseMovement = true;
    public bool attackEnabled = true;
    public bool poisonAttackEnabled = true;
    public bool intimidateAttackEnabled = true;
    public bool ultimateAttackEnabled = true;
    public bool autoAttack = true;

    private UIbutton attackUIbutton;
    private UIbutton poisonUIbutton;
    private UIbutton intimidateUIbutton;
    private UIbutton ultUIbutton;
    private UIbutton repairUIbutton;
    private UIbutton placeUIbutton;

    #endregion

    #region Unity Functions

    private void Awake()
    {
        _Resources = new List<GameObject>();
        _controller = gameObject.GetComponent<CharacterController>();
        _player = gameObject.GetComponent<Player>();
        GameObject tower = GameObject.FindGameObjectWithTag("Tower");
        _tower = tower.GetComponent<Tower>();
        _mainCamera = Camera.main;
        uiManager = ServiceLocator.Get<UIManager>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        poisonUIbutton = ServiceLocator.Get<UIManager>()?.poisonImg.GetComponent<UIbutton>();
        intimidateUIbutton = ServiceLocator.Get<UIManager>()?.intimidateImg.GetComponent<UIbutton>();
        ultUIbutton = ServiceLocator.Get<UIManager>()?.ultImg.GetComponent<UIbutton>();
        repairUIbutton = ServiceLocator.Get<UIManager>()?.repairButton.GetComponent<UIbutton>();
        placeUIbutton = ServiceLocator.Get<UIManager>()?.placeButton.GetComponent<UIbutton>();
        attackUIbutton = ServiceLocator.Get<UIManager>()?.basicAttackButton.GetComponent<UIbutton>();
    }

    private void Update()
    {
        

        if(_lockedOnEnemyGO)
        {
            if (_lockedOnEnemyGO.GetComponent<Enemy>().IsDead)
            {
                _lockedOnEnemyGO = null;
                _isTargetLockedOn = false;
            }
        }

        ActivateTargetLockedOn();

        if(!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            CalculateMovement();

        if (_isRepairing)
        {
            _isRepairing = _RepairBarricade.GetComponent<Barricade>().CheckRepairValid(transform);
        }
        UpdateUI();


        CheckMoveIndicatorActive();

        if(Input.GetMouseButtonDown(0))
        {
            CheckSpawnBarricade();
        }

        //if (Input.GetKeyDown(_PickUpButton) || CheckHoldDownClick("BarricadeSpawner"))
        //{
        //    if (!_isHoldingItem)
        //    {
        //        _Barricade = _player.DetectBarricadeSpawner();
        //        if (_Barricade == null)
        //            _isHoldingItem = false;
        //        else if (_Barricade.GetComponent<Barricade>().CanBePickedUp())
        //        {
        //            _Barricade.GetComponent<Barricade>().PickUp(gameObject);
        //            _isHoldingItem = true;
        //        }
        //    }
        //    else if (!CheckBarricadePickUp())
        //    {
        //        StartCoroutine(PlaceBarricade());
        //    }

        //}

        if (Input.GetKeyDown(_PickUpButton) || CheckHoldDownClick("ResourceSpawner"))
        {
            if(!_isDigging)
            {
                _isDigging = true;
                StartCoroutine(DiggingTrash());
            }

        }

        if (placeUIbutton.isButtonPressed || CheckHoldDownClick("Ground"))
        {
            if (_isHoldingItem && _Barricade != null)
            {
                _isHoldingItem = false;
                StartCoroutine(PlaceBarricade());
            }

        }
        if (_isHoldingItem)
            return;

        if (Input.GetKeyDown(_RepairButton) || CheckHoldDownClick("Barricade") || repairUIbutton.isButtonPressed)
        {
            _RepairBarricade = _player.DetectBarricade();
            if (_RepairBarricade == null)
                _isHoldingItem = false;
            else if (!_isRepairing)
            {
                _isRepairing = true;
                _RepairBarricade.GetComponent<Barricade>().inRangeRepair = true;
                StartCoroutine(_RepairBarricade.GetComponent<Barricade>().Repair());
            }
        }

        if (/*(Input.GetKeyDown(_AttackButton) && attackEnabled) ||*/ (autoAttack /*&& CheckCoolDownTimes()*/ && _isTargetLockedOn))
        {
            if (currentAttackCoolDown < Time.time)
            {
                StartCoroutine(_player.Attack());
                currentAttackCoolDown = Time.time + attackCoolDown;
            }
        }

        if ((Input.GetKeyDown(_PoisonAttackButton) || poisonUIbutton.isButtonPressed) && poisonAttackEnabled)
        {
            if (currentPoisonAttackCoolDown < Time.time)
            {
                StartCoroutine(_player.PoisonAttack());
                currentPoisonAttackCoolDown = Time.time + poisonAttackCoolDown;
            }
        }

        if (Input.GetKeyDown(_LockTargetButton))
        {
            if (_isTargetLockedOn && !_lockedOnEnemyGO)
            {
                _isTargetLockedOn = false;
            }

            if (_isTargetLockedOn)
            {
                GameObject prevTarget = _lockedOnEnemyGO;
                CheckTargetLockedOn();

                if (_lockedOnEnemyGO != null && prevTarget != _lockedOnEnemyGO)
                {
                    //Deselect
                    prevTarget?.GetComponent<Enemy>().SwitchOnTargetIndicator(false);
                    _lockedOnEnemyGO.GetComponent<Enemy>().SwitchOnTargetIndicator(true);
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

        if ((Input.GetKeyDown(_UltimateButton) || ultUIbutton.isButtonPressed) && ultimateAttackEnabled)
        {
            //_player.UltimateAttack();
            ultUIbutton.isButtonPressed = false;
            StartCoroutine(_player.UltimateAttack());
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

        if ((Input.GetKeyDown(_Intimidate) || intimidateUIbutton.isButtonPressed) && intimidateAttackEnabled)
        {
            if (currentIntimidateAttackCoolDown < Time.time)
            {
                if (_isTargetLockedOn)
                {
                    _player.IntimidateAttack(_lockedOnEnemyGO);
                }
                currentIntimidateAttackCoolDown = Time.time + intimidateAttackCoolDown;
            }
        }

        if (Vector3.Distance(_tower.transform.position, transform.position) < allowedRangeofResource && _Resources.Count > 0)
        {
            _tower.GetComponent<Tower>().fullHealth += 10.0f * currentTrashes;
            if (_tower.GetComponent<Tower>().fullHealth > 100.0f)
                _tower.GetComponent<Tower>().fullHealth = 100.0f;
            UIManager uiManager = ServiceLocator.Get<UIManager>();
            uiManager.UpdateTowerHealth(_tower.GetComponent<Tower>().fullHealth);

            foreach(GameObject trash in _Resources)
            {
                Destroy(trash);
            }
            _Resources.Clear();
            currentTrashes = 0;
        }
        //ActivateTargetLockedOn();

    }

    #endregion

    #region Ultility

    private IEnumerator DiggingTrash()
    {
        if (_isHoldingItem)
            yield return null;
        agent.isStopped = true;
        yield return new WaitForSeconds(diggingTime);
        _isDigging = false;
        agent.isStopped = false;
        if (limitOfHolding - 1 > currentTrashes)
        {
            _Resources.Add(_player.DetectResourceSpawner());
            currentTrashes++;
            foreach (GameObject trash in _Resources)
            {
                if (trash)
                {
                    if (trash.GetComponent<Resource>().CanBePickedUp())
                    {
                        trash.GetComponent<Resource>().Pickup(gameObject);
                    }
                }
            }

        }
        yield return null;
    }

    public void CheckMoveIndicatorActive()
    {
        List<GameObject> particles = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects("MoveIndicator");
        if (particles.Count > 0)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                if (Vector3.Distance(particles[i].transform.position, transform.position) < 1.0f)
                    ServiceLocator.Get<ObjectPoolManager>().RecycleObject(particles[i]);
                //else if (_isTargetLockedOn)
                //    Destroy(particles[i]);
            }
        }
    }

    public bool CheckHoldDownClick(string tagName)
    {
        if (Input.GetKey(_ClickMovementButton))
        {
            holdClickTime += Time.deltaTime;
            if (holdClickTime > holdClickTimeMax)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.CompareTag(tagName))
                    {
                        holdClickTime = 0.0f;
                        return true;
                    }
                }
            }
        }
        else
        {
            holdClickTime = 0.0f;
        }
        return false;
    }

    public bool CheckBarricadePickUp()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.CompareTag("BarricadeSpawner"))
            {
                return true;
            }
        }
        return false;
    }

    public void CheckSpawnBarricade()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            GameObject go = hit.transform.gameObject;
            if (go.CompareTag("BarricadeSpawner"))
            {
                go.GetComponent<BarricadeSpawner>().SpawnBarricade();
            }
        }
    }

    public bool CheckCoolDownTimes()
    {
        if (currentPoisonAttackCoolDown < Time.time && currentIntimidateAttackCoolDown < Time.time)
        {
            return true;
        }
        return false;
    }

    public bool CheckUIbuttonPressed()
    {
        if (!attackUIbutton.isButtonPressed && !ultUIbutton.isButtonPressed
            && !poisonUIbutton.isButtonPressed && !intimidateUIbutton.isButtonPressed
            && !placeUIbutton.isButtonPressed && !repairUIbutton.isButtonPressed)
            return true;
        return false;
    }

    public void CalculateMovement()
    {
        if (!_CanMove || _isDigging)
            return;

        if (isUsingMouseMovement)
        {
            //Movement
            agent.speed = moveSpeed;
            //Rotation
            Vector3 direction = agent.destination - transform.position;
            if (direction.magnitude > 0.0f)
            {
                Quaternion newDirection = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, newDirection, Time.deltaTime * turnSpeed);
            }

            if (Input.GetKeyDown(_ClickMovementButton))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit))
                {
                    if (/*(hit.transform.gameObject.CompareTag("Ground") || hit.transform.gameObject.CompareTag("PickUp")) &&*/ 
                        CheckUIbuttonPressed() && (hit.transform.gameObject.CompareTag("Ground")))
                    {
                        agent.isStopped = true;
                        agent.SetDestination(hit.point);
                        agent.isStopped = false;

                        //Deselect
                        _isTargetLockedOn = false;
                        _lockedOnEnemyGO?.GetComponent<Enemy>()?.SwitchOnTargetIndicator(false);
                        _lockedOnEnemyGO = null;

                        //Show MoveIndicator
                        ServiceLocator.Get<ObjectPoolManager>().RecycleAllObjects("MoveIndicator");
                        GameObject moveIndicator = ServiceLocator.Get<ObjectPoolManager>().GetObjectFromPool("MoveIndicator");
                        moveIndicator.transform.position = agent.destination;
                        moveIndicator.SetActive(true);
                        return;
                    }
                }
            }

            //Attack target
            if (_isTargetLockedOn)
            {
                if(_lockedOnEnemyGO)
                    agent.SetDestination(_lockedOnEnemyGO.transform.position);
                agent.isStopped = false;
                Vector3 look = agent.destination;
                look.y = transform.position.y;
                transform.LookAt(look);
                if (Vector3.Distance(transform.position, agent.destination) < _player.attackRange)
                    agent.isStopped = true;
                return;
            }
            return;
        }

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
        _Barricade?.GetComponent<Barricade>().PlaceBarricade();
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
                _lockedOnEnemyGO.GetComponent<Enemy>()?.SwitchOnTargetIndicator(false);
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
            {
                if(!hit.transform.gameObject.GetComponent<Enemy>().IsDead)
                {
                    _lockedOnEnemyGO = hit.transform.gameObject;
                }
                else
                {
                    _lockedOnEnemyGO = null;
                }
            }
            else
                _lockedOnEnemyGO = null;
        }
    }

    public void SwitchAutoLock(GameObject enemy)
    {
        _lockedOnEnemyGO?.GetComponent<Enemy>()?.SwitchOnTargetIndicator(false);

        _lockedOnEnemyGO = enemy;
        _isTargetLockedOn = true;
        _lockedOnEnemyGO?.GetComponent<Enemy>()?.SwitchOnTargetIndicator(true);
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

    #region UI
    public void UpdateUI()
    {
        float fill = (currentAttackCoolDown - Time.time) / attackCoolDown;
        fill = Mathf.Clamp(fill, 0.0f, 1.0f);
        uiManager.UpdateImage(DamageType.Normal, fill);

        fill = (currentPoisonAttackCoolDown - Time.time) / poisonAttackCoolDown;
        fill = Mathf.Clamp(fill, 0.0f, 1.0f);
        uiManager.UpdateImage(DamageType.Poison, fill);

        fill = (currentIntimidateAttackCoolDown - Time.time) / intimidateAttackCoolDown;
        fill = Mathf.Clamp(fill, 0.0f, 1.0f);
        uiManager.UpdateImage(DamageType.Intimidate, fill);

        fill = ((_player._ultimateCharge) / 100.0f) - 1.0f;
        fill = Mathf.Clamp(-fill, 0.0f, 1.0f);
        uiManager.UpdateImage(DamageType.Ultimate, fill);

        fill = (holdClickTime / holdClickTimeMax);
        fill = Mathf.Clamp(fill, 0.0f, 1.0f);
        if (fill < 0.5f)
            fill = 0.0f;
        uiManager.UpdateImage(DamageType.Loading, fill);

        uiManager.repairIcon.enabled = _isRepairing;
    }

    #endregion

    public void EnableAttack()
    {
        UIManager uiManager = ServiceLocator.Get<UIManager>();
        uiManager.attackImg.SetActive(true);
        attackEnabled = true;
    }
    public void EnablePoisonAttack()
    {
        UIManager uiManager = ServiceLocator.Get<UIManager>();
        uiManager.poisonImg.SetActive(true);
        poisonAttackEnabled = true;
    }
    public void EnableIntimidateAttack()
    {
        UIManager uiManager = ServiceLocator.Get<UIManager>();
        uiManager.intimidateImg.SetActive(true);
        intimidateAttackEnabled = true;
    }
    public void EnableUltAttack()
    {
        UIManager uiManager = ServiceLocator.Get<UIManager>();
        uiManager.ultImg.SetActive(true);
        ultimateAttackEnabled = true;
    }
}