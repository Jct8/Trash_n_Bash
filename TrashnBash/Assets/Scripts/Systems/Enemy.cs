using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, ICharacterAction
{
    public Action killed;
    public Rigidbody rigid;

    [Header("Unity Stuff")]
    public Image healthBar;

    public GameObject healthBarGO;

    private DataLoader _DataLoader;
    private JsonDataSource _EnemyData;
    private NavMeshAgent _Agent;
    private WayPointManager.Path _Path;

    public GameObject player;
    public GameObject popUp;
    private GameObject _targetIndicator;
    private GameObject _ObjectofBarricade;
    public float fullHealth;
    public int _CurrentWayPoint = 0;
    private bool _IsDead = false;
    public Detect _Detect { get; set; }
    public Order _Order { get; set; }
    public bool _isDetected = false;

    //public string _DataSource;

    [SerializeField]
    private string _Name;
    [SerializeField]
    private float _Attack;
    [SerializeField]
    private float _Health;
    [SerializeField]
    private float _Money;
    [SerializeField]
    private float _Speed;
    [SerializeField]
    private float _AttackCoolTime = 3.0f;
    private float _enemyAttackRange = 2.0f;
    private float _EndDistance = 3.0f;
    private float _InsideofRange = 200.0f;
    private float _MaximumAngle = 45.0f;
    private float _MaximumDistance = 7.0f;
    private float _poisonDamage = 0.0f;
    private float _poisonTotalTime = 0.0f;
    private float _poisonTickTime = 0.0f;
    private float _poisonCurrentDuration = 0.0f;
    private bool _isPoisoned = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        _ObjectofBarricade = GameObject.FindGameObjectWithTag("Barricade");
    }

    private void OnEnable()
    {
        player = ServiceLocator.Get<LevelManager>().playerInstance;
    }

    public void Initialize(WayPointManager.Path path, Action Recycle)
    {
        _Path = path;
        killed += Recycle;
        //_DataLoader = ServiceLocator.Get<DataLoader>();
        //_EnemyData = _DataLoader.GetDataSourceById(_DataSource) as JsonDataSource;

        //_Name = System.Convert.ToString(_EnemyData.DataDictionary["Name"]);
        //_Attack = System.Convert.ToSingle(_EnemyData.DataDictionary["Attack"]);
        //_Health = System.Convert.ToSingle(_EnemyData.DataDictionary["Health"]);
        //_Money = System.Convert.ToSingle(_EnemyData.DataDictionary["Money"]);

        _Detect = Detect.None;
        _Order = Order.Tower;

        _Agent = GetComponent<NavMeshAgent>();
        _IsDead = false;
        fullHealth = _Health;
        healthBar.fillAmount = fullHealth / _Health;
        rigid = gameObject.GetComponent<Rigidbody>();
        _targetIndicator = transform.Find("TargetIndicator").gameObject;
        gameObject.GetComponent<Enemy>().SwitchOnTargetIndicator(false);
        _isDetected = false;
    }

    void Update()
    {
        healthBarGO.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward,Camera.main.transform.up);
        if (player == null)
            return;
            //player = GameObject.FindGameObjectWithTag("Player");
        if (_ObjectofBarricade == null)
            _ObjectofBarricade = GameObject.FindGameObjectWithTag("Barricade");

        if (!_IsDead)
        {
            if (_isPoisoned)
            {
                CheckPoison();
            }

            //UpdateAnimation();
            Transform _Desination = _Path.WayPoints[_CurrentWayPoint];
            if (_Order == Order.Tower)
            {
                _Agent.SetDestination(_Desination.position);
                if ((Vector3.Distance(transform.position, _Desination.position) < _EndDistance) && (_Detect == Detect.Detected || _Detect == Detect.None))
                {
                    if (_CurrentWayPoint == 2)
                    {
                        if (_AttackCoolTime <= 0.0f)
                        {
                            StartCoroutine("Attack");
                            _AttackCoolTime = 3.0f;
                        }
                    }
                    else
                    {
                        _CurrentWayPoint++;
                    }
                }
                if (Vector3.Distance(transform.position, _ObjectofBarricade.transform.position) <= 4.0f
                    && _ObjectofBarricade.GetComponent<Barricade>().isAlive == true && _ObjectofBarricade.GetComponent<Barricade>().isPlaced == true)
                {
                    _Order = Order.Barricade;
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
                    if ((Vector3.Distance(transform.position, player.transform.position) <= _enemyAttackRange))
                    {
                        if (_AttackCoolTime <= 0.0f)
                        {
                            StartCoroutine("Attack");
                            _AttackCoolTime = 3.0f;
                        }
                    }
                    else
                    {
                        _Agent.isStopped = false;
                    }
                }
            }
            else if (_Order == Order.Back)
            {
                _Agent.isStopped = false;
                _Desination = _Path.WayPoints[0];
                _Agent.SetDestination(_Desination.position);
                if ((Vector3.Distance(transform.position, _Desination.position) < _EndDistance))
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
                if (Vector3.Distance(transform.position, _ObjectofBarricade.transform.position) <= 1.5f && _ObjectofBarricade.GetComponent<Barricade>().isPlaced == true)
                {
                    if (_AttackCoolTime <= 0.0f)
                    {
                        BarricadeAttack();
                        _AttackCoolTime = 3.0f;
                    }
                }

            }
            Detection();
            rigid.velocity = Vector3.zero;
        }
        else
        {
            rigid.velocity = Vector3.zero;
        }

        _AttackCoolTime -= Time.deltaTime;
    }

    public void Detection()
    {
        if (_Order == Order.Fight) //Justin : added to follow and attack player without stopping
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

    public void Alive()
    {
        _Agent.isStopped = false;
        _IsDead = false;
        _Detect = Detect.None;
        _Order = Order.Tower;
        _CurrentWayPoint = 0;
        fullHealth = _Health;
        healthBar.fillAmount = fullHealth / _Health;
        rigid.velocity = Vector3.zero;
        _isDetected = false;
        _isPoisoned = false;
    }

    public void TakeDamage(float Dmg, bool isHero)
    {
        fullHealth -= Dmg;

        if(popUp)
        {
            PopingDamageText(Dmg);
        }

        healthBar.fillAmount = fullHealth / _Health;

        if (_Detect == Detect.Detected && isHero == true)
        {
            GameObject[] list = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in list)
            {
                if(enemy.GetComponent<Enemy>()._Order == Order.Fight)
                {
                    _isDetected = false;
                    break;
                }
                else
                {
                    _isDetected = true;
                }
            }
            if(_isDetected)
                _Order = Order.Fight;
        }
        else
            _Detect = Detect.Detected;

        if (fullHealth <= 0.0f)
        {
            StartCoroutine("DeathAnimation");
            if (!_IsDead)
                player.GetComponent<Player>().IncrementUltCharge();
            _IsDead = true;
            _isPoisoned = false;
        }
    }

    private void PopingDamageText(float dmg)
    {
        Instantiate(popUp, transform.position, Quaternion.identity, transform);
    }

    private void CheckPoison()
    {
        if (_poisonCurrentDuration < Time.time)
        {
            _poisonCurrentDuration = Time.time + _poisonTickTime;
            TakeDamage(_poisonDamage, true);
        }
        if (_poisonTotalTime < Time.time)
        {
            _isPoisoned = false;
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
    }

    public void BarricadeAttack()
    {
        _ObjectofBarricade = GameObject.FindGameObjectWithTag("Barricade");
        _Agent.isStopped = true;
        if(_ObjectofBarricade.GetComponent<Barricade>().isAlive == true )
        {
            _ObjectofBarricade?.GetComponent<Barricade>().TakeDamage(1.0f);
        }
        else
        {
            _Order = Order.Tower;
            _Agent.isStopped = false;
        }
    }

    public IEnumerator Attack()
    {
        _Agent.isStopped = true;
        GameObject _player = GameObject.FindGameObjectWithTag("Player");
        GameObject _tower = GameObject.FindGameObjectWithTag("Tower");
        yield return new WaitForSeconds(3.0f);
        if (FrontAttack(_tower.transform))
        {
            if (this._Name == "Skunks_1")
                _tower.GetComponent<Tower>().TakeDamage(3.0f);
            else if(this._Name == "Rats_1")
                _tower.GetComponent<Tower>().TakeDamage(1.0f);
        }
        else if (FrontAttack(_player.transform))
        {
            _player.GetComponent<Player>().TakeDamage(_Attack, false);
            ServiceLocator.Get<UIManager>().StartCoroutine("HitAnimation");
        }
        if (_Order != Order.Fight)
        {
            _Order = Order.Back;
        }
        yield return null;
    }

    bool FrontAttack(Transform target)
    {
        Vector3 Coneforward = transform.TransformDirection(Vector3.forward);
        Vector3 ConeToTarget = target.position - transform.position;
        ConeToTarget.Normalize();
        float _Distance = Vector3.Distance(transform.position, target.transform.position);

        if ((Vector3.Angle(ConeToTarget, Coneforward) < _MaximumAngle) && (_Distance <= _MaximumDistance))
        {
            return true;
        }

        return false;
    }

    public void SwitchOnTargetIndicator(bool turnOn)
    {
        _targetIndicator.SetActive(turnOn);
    }

    public IEnumerator DeathAnimation()
    {
        _Agent.isStopped = true;
        _AttackCoolTime = 3.0f;
        yield return new WaitForSeconds(1.0f);
        ServiceLocator.Get<LevelManager>().IncreaseEnemyDeathCount(1);
        killed?.Invoke();
        yield return null;
    }

    public void UpdateAnimation()
    {
        throw new NotImplementedException();
    }
}
