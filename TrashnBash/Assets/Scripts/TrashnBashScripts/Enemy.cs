using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, ICharacterAction
{
    #region Variables
    [Header("UI Health")]
    public Image healthBar;
    public Image CooltimeBar;
    public GameObject healthBarGO;
    public GameObject CoolTimeGO;
    public GameObject popUp;
    public GameObject pickUp;
    public GameObject poison;
    public GameObject hitEffect;

    private GameObject player;
    private GameObject _targetIndicator;
    private GameObject _ObjectofBarricade;
    private DataLoader _DataLoader;
    private JsonDataSource _EnemyData;
    private EnemySpawner.EnemyPath _Path;
    public NavMeshAgent _Agent;

    public Order _Order { get; set; }

    public float fullHealth;
    public float stunTime = 0.0f;
    private int _CurrentWayPoint = 0;
    private bool _isDetected = false;
    private bool _IsDead = false;
    private bool _IsStolen = false;
    private bool _IsAttacked = false;
    private bool _isPoisoned = false;
    public string _DataSource;

    public AudioClip attackEffect;
    public AudioClip poisonedEffect;
    private AudioSource audioSource;

    public string Name { get { return _Name; } private set { } }
    public bool IsDead { get { return _IsDead; } set { _IsDead = value; } }
    [Header("Enemy Status")]
    public float _Health = 1.0f;
    [SerializeField] private string _Name;
    [SerializeField] private float _Attack = 1.0f;
    [SerializeField] private float _Money;
    [SerializeField] private float _Speed;
    [SerializeField] private float _AttackCoolTime = 3.0f;
    [SerializeField] private float _ObjectDetectionRange = 3.0f;
    [SerializeField] private float _DropRate = 0.5f;
    [SerializeField] private float _enemyAttackRange = 4.0f;
    [SerializeField] private float _waitForsecondOfCrows = 1.0f;

    private float _EndDistance = 3.0f;
    private float _MaximumAngle = 45.0f;
    private float _MaximumDistance = 5.0f;
    private float _distanceToBarricade;
    private float _poisonDamage = 0.0f;
    private float _poisonTotalTime = 0.0f;
    private float _poisonTickTime = 0.0f;
    private float _poisonCurrentDuration = 0.0f;
    private int numOfRats = 0;
    
    [Header("Etc")]
    public Rigidbody rigid;
    public Action killed;
    IEnemyAbilities enemyAbilities;

    private bool _barricadeAlive;
    private bool _barricadePlaced;

    #endregion

    #region UnityFunctions
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        CooltimeBar.fillAmount = 0;
        enemyAbilities = GetComponent<IEnemyAbilities>();
    }

    private void OnEnable()
    {
        player = ServiceLocator.Get<LevelManager>().playerInstance;
    }

    void Update()
    {

        healthBarGO.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
        CoolTimeGO.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
        numOfRats = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects(_Name).Count;
        Transform _Desination = _Path.WayPoints[_CurrentWayPoint];
        if (player == null || _IsDead)
        {
            _Agent.isStopped = true;
            return;
        }
        if (!_IsDead)
        {
            if (_targetIndicator.activeSelf)
                _Order = Order.Fight;
            

            enemyAbilities.PoisonAOE(player);


            if (_isPoisoned)
                CheckPoison();

            if (_Order == Order.Stunned)
            {
                _Agent.isStopped = true;
                if (stunTime < Time.time)
                {
                    _Agent.isStopped = false;
                    _Order = Order.Fight;
                }
                return;
            }
            else if (_Order == Order.Tower)
            {
                _Agent.SetDestination(_Desination.position);
                enemyAbilities.Flying(_Desination, _Order);
                if ((isInRangeOfWayPoint(_Desination, _EndDistance)))
                {

                    if (_CurrentWayPoint == _Path.WayPoints.Count - 1)
                    {


                        if (_Name == "Crows")
                        {
                            StartCoroutine("wait");
                        }
                        else
                        {
                            if (ChargingCoolDown())
                            {
                                StartCoroutine("TowerAttack");
                            }
                        }

                    }
                    else
                    {
                        _CurrentWayPoint++;
                    }

                }

                GameObject[] barricades = GameObject.FindGameObjectsWithTag("Barricade");
                for (int i = 0; i < barricades.Length; i++)
                {
                    if (_ObjectofBarricade)
                        break;
                    if (Vector3.Distance(barricades[i].gameObject.transform.position, transform.position) < _enemyAttackRange)
                    {
                        _ObjectofBarricade = barricades[i];
                        break;
                    }
                }
                if (_Name != "Crows")
                {
                    if (_ObjectofBarricade)
                    {
                        if (_ObjectofBarricade.GetComponent<Barricade>().isAlive)
                        {
                            if (isCloseToBarricade(_ObjectDetectionRange))
                                _Order = Order.Barricade;
                        }
                        else
                        {
                            _ObjectofBarricade = null;
                            for (int i = 0; i < barricades.Length; i++)
                            {
                                if (Vector3.Distance(barricades[i].gameObject.transform.position, transform.position) < _enemyAttackRange)
                                {
                                    _ObjectofBarricade = barricades[i];
                                    break;
                                }
                            }
                        }
                    }
                }


            }
            else if (_Order == Order.Fight)
            {
                LookAt(player.transform.position, player);
                if (isInRangeOfWayPoint(player.transform, _enemyAttackRange))
                {
                    if (ChargingCoolDown())
                        StartCoroutine("Attack");
                }
                else
                {
                    CooltimeBar.fillAmount = 0;
                    _IsAttacked = false;
                }
            }
            else if (_Order == Order.Barricade)
            {
                LookAt(_ObjectofBarricade.transform.position, _ObjectofBarricade);
                if (isCloseToBarricade(_enemyAttackRange))
                {
                    if (ChargingCoolDown())
                        BarricadeAttack();
                }
                if (!_ObjectofBarricade.GetComponent<Barricade>().isAlive)
                {
                    _ObjectofBarricade = null;
                    _Order = Order.Tower;
                }
            }
            else if (_Order == Order.Back)
            {
                //if(_Name == "Crows")
                //{
                //    _CurrentWayPoint = 0;
                //}
                _Agent.isStopped = false;
                _Desination = _Path.WayPoints[0];
                _Agent.SetDestination(_Desination.position);
                enemyAbilities.Flying(_Desination, _Order);
                if (isInRangeOfWayPoint(_Desination, _EndDistance))
                    killed?.Invoke();
            }

            if (_IsAttacked)
            {
                ChargingCoolDown();
            }
        }
    }
    #endregion

    #region Initiliaztion

    public void Initialize(EnemySpawner.EnemyPath path, Action Recycle)
    {
        poison.SetActive(_isPoisoned);
        _Path = path;
        killed += Recycle;
        _DataLoader = ServiceLocator.Get<DataLoader>();
        _EnemyData = _DataLoader.GetDataSourceById(_DataSource) as JsonDataSource;

        _Name = System.Convert.ToString(_EnemyData.DataDictionary["Name"]);

        _Order = Order.Tower;

        _Agent = GetComponent<NavMeshAgent>();
        _Agent.speed = _Speed;
        _IsDead = false;
        fullHealth = _Health;
        healthBar.fillAmount = fullHealth / _Health;
        CooltimeBar.fillAmount = 0;
        rigid = gameObject.GetComponent<Rigidbody>();
        _targetIndicator = transform.Find("TargetIndicator").gameObject;

        gameObject.GetComponent<Enemy>().SwitchOnTargetIndicator(false);
        _isDetected = false;
        _IsAttacked = false;
        _isPoisoned = false;
    }

    public void Alive()
    {
        _Agent.isStopped = false;
        _IsDead = false;
        _Order = Order.Tower;
        _CurrentWayPoint = 0;
        fullHealth = _Health;
        healthBar.fillAmount = fullHealth / _Health;
        CooltimeBar.fillAmount = 0;
        rigid.velocity = Vector3.zero;
        _isDetected = false;
        _isPoisoned = false;
        _IsStolen = false;
        _IsAttacked = false;
        poison.SetActive(_isPoisoned);
        player = ServiceLocator.Get<LevelManager>().playerInstance;
    }

    #endregion

    #region Check and Ultility Functions
    private bool ChargingCoolDown()
    {
        CooltimeBar.fillAmount += 1 / (_AttackCoolTime * 60.0f);
        if (CooltimeBar.fillAmount >= 1)
        {
            CooltimeBar.fillAmount = 0;
            return true;
        }
        return false;
    }

    public void SwitchOnTargetIndicator(bool turnOn)
    {
        _targetIndicator.SetActive(turnOn);
    }

    public void SwitchEnemyDead(bool active)
    {
        _IsDead = active;
    }

    private bool isInRangeOfWayPoint(Transform destination, float range)
    {
        // Range is enemy's area, it could be enemy's attack range, or enemy's width
        // if enemy's area reachs to inside of distance, they stop moving
        float distance = Vector3.Distance(transform.position, destination.position);
        return (distance <= range) ? _Agent.isStopped = true : _Agent.isStopped = false;
    }

    private bool isCloseToBarricade(float range)
    {
        float distance = Vector3.Distance(transform.position, _ObjectofBarricade.transform.position);
        _barricadeAlive = _ObjectofBarricade.GetComponent<Barricade>().isAlive;
        _barricadePlaced = _ObjectofBarricade.GetComponent<Barricade>().isPlaced;

        if (distance < _enemyAttackRange && _barricadeAlive && _barricadePlaced)
            _Agent.isStopped = true;
        else
            _Agent.isStopped = false;

        return _Agent.isStopped;
    }

    private void LookAt(Vector3 obj, GameObject go)
    {
        _Agent.SetDestination(go.transform.position);
        obj.y = transform.position.y;
        transform.LookAt(obj);
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(_waitForsecondOfCrows);
        if (ChargingCoolDown())
        {
            StartCoroutine("TowerAttack");
        }

    }

    #endregion

    #region TakeDamage
    public void TakeDamage(float Dmg, bool isHero, DamageType type)
    {
        if (_IsDead)
            return;

        enemyAbilities.PlayDead(player);
        fullHealth -= Dmg;

        popUp.GetComponent<TextMesh>().text = Dmg.ToString();

        switch (type)
        {
            case DamageType.Normal:
                {
                    popUp.GetComponent<TextMesh>().color = new Color(1.0f, 0.0f, 0.0f);
                    GameObject hit = Instantiate(hitEffect, transform.position, Quaternion.identity) as GameObject;
                    break;
                }
            case DamageType.Poison:
                {
                    popUp.GetComponent<TextMesh>().color = new Color(0.0f, 1.0f, 0.0f);
                    break;
                }
            case DamageType.Ultimate:
                {
                    popUp.GetComponent<TextMesh>().color = new Color(0.0f, 0.0f, 1.0f);
                    break;
                }
        }
        Instantiate(popUp, transform.position, Camera.main.transform.rotation, transform);

        healthBar.fillAmount = fullHealth / _Health;

        if (isHero)
        {
            if (_Name == "Rats" && numOfRats <= 3)
            {
                enemyAbilities.GroupAttack();
            }

            GameObject[] list = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in list)
            {
                Order order = enemy.GetComponent<Enemy>()._Order;
                if (order == Order.Fight)
                {
                    _isDetected = false;
                    break;
                }
                else
                {
                    _isDetected = true;
                }
            }
            if (_isDetected)
                _Order = Order.Fight;
        }

        if (fullHealth <= 0.0f)
        {
            if (_IsStolen)
            {
                float randomNumber = UnityEngine.Random.Range(0.0f, 1.0f);
                if (randomNumber > _DropRate)
                {
                    Instantiate(pickUp, transform.position, Quaternion.identity);
                }
                _IsStolen = false;
            }
            if(gameObject.activeInHierarchy)
            {
                StartCoroutine("DeathAnimation");
            }
            if (!_IsDead)
                player.GetComponent<Player>().IncrementUltCharge();

            _IsDead = true;
            _isPoisoned = false;
            _IsAttacked = false;
        }
    }

    private void CheckPoison()
    {
        if (_poisonCurrentDuration < Time.time)
        {
            _poisonCurrentDuration = Time.time + _poisonTickTime;
            TakeDamage(_poisonDamage, true, DamageType.Poison);
        }
        if (_poisonTotalTime < Time.time)
        {
            _isPoisoned = false;
            poison.SetActive(_isPoisoned);
            return;
        }
    }

    public void SetPoison(float damage, float tickTime, float totalTime)
    {
        if (_isPoisoned)
        {
            return;
        }
        _poisonDamage = damage;
        _poisonTickTime = tickTime;
        _poisonCurrentDuration = Time.time;
        _poisonTotalTime = Time.time + totalTime;
        _isPoisoned = true;
        audioSource.PlayOneShot(poisonedEffect, 0.2f);
        poison.SetActive(_isPoisoned);
    }
    #endregion

    #region Attacks

    public void BarricadeAttack()
    {
        //_ObjectofBarricade = GameObject.FindGameObjectWithTag("Barricade");
        _Agent.isStopped = true;
        _IsAttacked = true;
        if (_ObjectofBarricade?.GetComponent<Barricade>().isAlive == true)
        {
            _ObjectofBarricade?.GetComponent<Barricade>().TakeDamage(_Attack);
        }
        else
        {
            _Order = Order.Tower;
            _ObjectofBarricade = null;
            _Agent.isStopped = false;
            _IsAttacked = false;
        }
    }

    public IEnumerator Attack()
    {
        _IsAttacked = true;
        player = ServiceLocator.Get<LevelManager>().playerInstance;
        if (FrontAttack(player.transform) && !_IsDead)
        {
            player.GetComponent<Player>().TakeDamage(_Attack, false, DamageType.Enemy);
            ServiceLocator.Get<UIManager>().StartCoroutine("HitAnimation");
        }
        audioSource.PlayOneShot(attackEffect, 0.7f);
        if (_Order != Order.Fight && !ServiceLocator.Get<LevelManager>().isTutorial)
        {
            _Order = Order.Back;
        }
        CooltimeBar.fillAmount = 0;
        _IsAttacked = false;
        yield return null;
    }

    public IEnumerator TowerAttack()
    {
        _IsAttacked = true;
        GameObject _tower = ServiceLocator.Get<LevelManager>().towerInstance;
        _tower.GetComponent<Tower>().TakeDamage(_Attack);
        _IsStolen = true;
        audioSource.PlayOneShot(attackEffect, 0.7f);
        if (_Order != Order.Fight && !ServiceLocator.Get<LevelManager>().isTutorial)
            _Order = Order.Back;
        CooltimeBar.fillAmount = 0;
        _IsAttacked = false;
        yield return null;
    }

    bool FrontAttack(Transform target)
    {
        Vector3 Coneforward = transform.TransformDirection(Vector3.forward);
        Vector3 ConeToTarget = target.position - transform.position;
        ConeToTarget.Normalize();
        float _Distance = Vector3.Distance(transform.position, target.transform.position);

        if ((Vector3.Angle(ConeToTarget, Coneforward) < _MaximumAngle) && (_Distance < _MaximumDistance))
        {
            return true;
        }

        return false;
    }
    #endregion

    #region Animations

    public IEnumerator DeathAnimation()
    {
        _Agent.isStopped = true;
        yield return new WaitForSeconds(0.5f);
        ServiceLocator.Get<LevelManager>().IncreaseEnemyDeathCount(1);
        killed?.Invoke();
        yield return null;
    }

    public void UpdateAnimation()
    {
        throw new NotImplementedException();
    }
    #endregion
}
