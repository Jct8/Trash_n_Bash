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
    private NavMeshAgent _Agent;
    private WayPointManager.Path _Path;


    public Detect _Detect { get; set; }
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

    [Header("Enemy Status")]
    [SerializeField] private string _Name;
    [SerializeField] private float _Attack = 1.0f;
    [SerializeField] private float _Health = 1.0f;
    [SerializeField] private float _Money;
    [SerializeField] private float _Speed;
    [SerializeField] private float _AttackCoolTime = 3.0f;
    [SerializeField] private float _ObjectDetectionRange = 3.0f;
    [SerializeField] private float _DropRate = 0.5f;
    [SerializeField] private float _enemyAttackRange = 2.5f;

    private float _EndDistance = 3.0f;
    private float _InsideofRange = 200.0f;
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
            rigid.velocity = Vector3.zero;
            return;
        }

        enemyAbilities.PoisonAOE(player);
        if (_isPoisoned)
        {
            CheckPoison();
        }

        //UpdateAnimation();
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
            float distanceToTower = Vector3.Distance(transform.position, _Desination.position);
            if ((distanceToTower < _EndDistance) && (_Detect == Detect.Detected || _Detect == Detect.None))
            {
                if (_CurrentWayPoint == _Path.WayPoints.Count - 1)
                {
                    if(distanceToTower <= _enemyAttackRange)
                    {
                        _Agent.isStopped = true;
                    }
                    else
                    {
                        _Agent.isStopped = false;
                    }

                    if (ChargingCoolDown())
                    {
                        StartCoroutine("TowerAttack");
                    }
                }
                else
                {
                    _CurrentWayPoint++;
                }
            }
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _ObjectDetectionRange);
            foreach (var hit in hitColliders)
            {
                if (hit.CompareTag("Barricade"))
                {
                    _ObjectofBarricade = hit.gameObject;
                    break;
                }
            }
            if (_ObjectofBarricade)
            {
                _distanceToBarricade = Vector3.Distance(transform.position, _ObjectofBarricade.transform.position);
                if (_distanceToBarricade <= _ObjectDetectionRange 
                    && _ObjectofBarricade.GetComponent<Barricade>().isAlive == true 
                    && _ObjectofBarricade.GetComponent<Barricade>().isPlaced == true)
                {
                    _Order = Order.Barricade;
                }
            }
        }
        else if (_Order == Order.Fight)
        {
            if (_Detect == Detect.Attack)
            {
                _Agent.SetDestination(player.transform.position);
                Vector3 newtarget = player.transform.position;
                newtarget.y = transform.position.y;
                transform.LookAt(newtarget);
                if ((Vector3.Distance(transform.position, player.transform.position) < _enemyAttackRange))
                {
                    _Agent.isStopped = true;
                    if (ChargingCoolDown())
                    {
                        StartCoroutine("Attack");
                    }
                }
                else
                {
                    _Agent.isStopped = false;
                    CooltimeBar.fillAmount = 0;
                    _IsAttacked = false;
                }
            }
        }
        else if (_Order == Order.Back)
        {
            _Agent.isStopped = false;
            _Desination = _Path.WayPoints[0];
            _Agent.SetDestination(_Desination.position);
            float distanceToEscape = Vector3.Distance(transform.position, _Desination.position);
            if ((distanceToEscape < _EndDistance))
            {
                killed?.Invoke();
            }
        }
        else if (_Order == Order.Barricade)
        {
            _Agent.SetDestination(_ObjectofBarricade.transform.position);
            Vector3 targetToBarricade = _ObjectofBarricade.transform.position;
            targetToBarricade.y = transform.position.y;
            transform.LookAt(_ObjectofBarricade.transform);
            _distanceToBarricade = Vector3.Distance(transform.position, _ObjectofBarricade.transform.position);
            if (_distanceToBarricade <= 1.5f && _ObjectofBarricade.GetComponent<Barricade>().isPlaced == true)
            {
                _Agent.isStopped = true;
                if (ChargingCoolDown())
                {
                    BarricadeAttack();
                }
            }

        }
        Detection();

        if (_IsAttacked)
        {
            ChargingCoolDown();
        }
    }
    #endregion

    #region Initiliaztion

    public void Initialize(WayPointManager.Path path, Action Recycle)
    {
        poison.SetActive(_isPoisoned);
        _Path = path;
        killed += Recycle;
        _DataLoader = ServiceLocator.Get<DataLoader>();
        _EnemyData = _DataLoader.GetDataSourceById(_DataSource) as JsonDataSource;

        _Name = System.Convert.ToString(_EnemyData.DataDictionary["Name"]);
        //_Attack = System.Convert.ToSingle(_EnemyData.DataDictionary["Attack"]);
        //_Health = System.Convert.ToSingle(_EnemyData.DataDictionary["Health"]);
        //_Money = System.Convert.ToSingle(_EnemyData.DataDictionary["Money"]);

        _Detect = Detect.None;
        _Order = Order.Tower;

        _Agent = GetComponent<NavMeshAgent>();
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
        _Detect = Detect.None;
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

    public void Detection()
    {
        if (_Order == Order.Fight)
        {
            _Detect = Detect.Attack;
        }
        else if ((_Detect != Detect.Attack || _Detect != Detect.Detected) &&
            Vector3.Distance(_Path.WayPoints[2].transform.position, transform.position) < _InsideofRange)
        {
            _Detect = Detect.Detected;
        }
        else
        {
            _Detect = Detect.None;
        }
    }

    public void SwitchOnTargetIndicator(bool turnOn)
    {
        _targetIndicator.SetActive(turnOn);
    }
    #endregion

    #region TakeDamage
    public void TakeDamage(float Dmg, bool isHero, DamageType type)
    {
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

        if (_Detect == Detect.Detected && isHero == true)
        {
            if(_Name == "Rats" &&  numOfRats <= 3)
            {
                enemyAbilities.GroupAttack();
            }

            GameObject[] list = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in list)
            {
                if (enemy.GetComponent<Enemy>()._Order == Order.Fight)
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
        else
            _Detect = Detect.Detected;

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
            StartCoroutine("DeathAnimation");
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
        if (FrontAttack(player.transform))
        {
            player.GetComponent<Player>().TakeDamage(_Attack, false, DamageType.Enemy);
            ServiceLocator.Get<UIManager>().StartCoroutine("HitAnimation");
        }
        audioSource.PlayOneShot(attackEffect, 0.7f);
        if (_Order != Order.Fight)
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
        if (_Order != Order.Fight)
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
