using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, ICharacterAction
{
    public Action _Killed;
    public GameObject _Player;
    public Rigidbody _Rigid;
    private DataLoader _DataLoader;
    private JsonDataSource _EnemyData;
    private NavMeshAgent _Agent;
    private WayPointManager.Path _Path;

    public float _FullHealth;
    public int _CurrentWayPoint = 0;
    private bool _IsDead = false;
    public Detect _Detect { get; set; }
    public Order _Order { get; set; }
    public float _AttackCoolTime;

    public string _DataSource;
    public string _Name;
    public float _Attack;
    public float _Speed;
    public float _Health;
    public float _Money;

    public float _Stop = 0.0f;
    
    private void Start()
    {
        _Player = GameObject.FindGameObjectWithTag("Player");
    }

    private float poisonDamage = 0.0f;
    private bool isPoisoned = false;
    private float poisonTotalTime = 0.0f;
    private float poisonTickTime = 0.0f;
    private float poisonCurrentDuration = 0.0f;

    public void Initialize(WayPointManager.Path path, Action Recycle)
    {
        _Path = path;
        _Killed += Recycle;
        _DataLoader = ServiceLocator.Get<DataLoader>();
        _EnemyData = _DataLoader.GetDataSourceById(_DataSource) as JsonDataSource;
        _IsDead = false;
        _Agent = GetComponent<NavMeshAgent>();
        _Detect = Detect.None;
        _Order = Order.Tower;
        _Name = System.Convert.ToString(_EnemyData.DataDictionary["Name"]);
        _Attack = System.Convert.ToSingle(_EnemyData.DataDictionary["Attack"]);
        _Health = System.Convert.ToSingle(_EnemyData.DataDictionary["Health"]);
        _Money = System.Convert.ToSingle(_EnemyData.DataDictionary["Money"]);
        _Agent.speed = _Speed;
        _FullHealth = _Health;
        _Rigid = gameObject.GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        if(_Player == null)
            _Player = GameObject.FindGameObjectWithTag("Player");

        if (!_IsDead)
        {
            if (isPoisoned)
            {
                CheckPoison();
            }

            //UpdateAnimation();
            Transform _Desination = _Path.WayPoints[_CurrentWayPoint];
            if(_Order == Order.Tower)
            {
                _Agent.SetDestination(_Desination.position);
                if ((Vector3.Distance(transform.position, _Desination.position) < 2.0f) && (_Detect == Detect.Detected || _Detect == Detect.None))
                {
                    if (_CurrentWayPoint == 2)
                    {
                        _Agent.speed = _Stop;
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
            }
            else if(_Order == Order.Fight)
            {
                if (_Detect == Detect.Attack)
                {
                    _Agent.SetDestination(_Player.transform.position);
                    if ((Vector3.Distance(transform.position, _Player.transform.position) <= 100.0f))
                    {
                        _Agent.speed = _Stop;
                        if (_AttackCoolTime <= 0.0f)
                        {
                            StartCoroutine("Attack");
                            _AttackCoolTime = 3.0f;
                        }
                    }
                    else
                    {
                        _Agent.speed = _Speed;
                    }
                }
            }
            else if(_Order == Order.Back)
            {
                _Agent.speed = _Speed;
                _Desination = _Path.WayPoints[0];
                _Agent.SetDestination(_Desination.position);
                if((Vector3.Distance(transform.position, _Desination.position) < 2.0f))
                {
                    _Killed?.Invoke();
                }
            }
            Detection();
            _Rigid.velocity = Vector3.zero;
        }
        else
        {
            _Rigid.velocity = Vector3.zero;
        }

        _AttackCoolTime -= Time.deltaTime;
    }

    public void Detection()
    {
        if ((_Detect != Detect.Attack || _Detect != Detect.Detected) && 
            Vector3.Distance(_Path.WayPoints[2].transform.position, transform.position) < 200.0f)
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
        _IsDead = false;
        _Detect = Detect.None;
        _Order = Order.Tower;
        _CurrentWayPoint = 0;
        _FullHealth = _Health;
        _Rigid.velocity = Vector3.zero;
    }

    public void TakeDamage(float Dmg, bool isHero)
    {
        _FullHealth -= Dmg;

        if (_Detect == Detect.Detected && isHero == true)
        {
            _Detect = Detect.Attack;
            _Order = Order.Fight;
        }
        else
            _Detect = Detect.Detected;

        if(_FullHealth <= 0.0f)
        {
            _IsDead = true;
            isPoisoned = false;
            StartCoroutine("DeathAnimation");
        }
    }

    private void CheckPoison()
    {
        if (poisonCurrentDuration < Time.time)
        {
            poisonCurrentDuration = Time.time + poisonTickTime;
            TakeDamage(poisonDamage, true);
        }
        if (poisonTotalTime < Time.time)
        {
            isPoisoned = false;
            return;
        }
    }

    public void SetPoison(float damage,float tickTime, float totalTime)
    {
        if (isPoisoned)
        {
            return;
        }
        poisonDamage = damage;
        poisonTickTime = tickTime;
        poisonCurrentDuration = Time.time;
        poisonTotalTime = Time.time + totalTime;
        isPoisoned = true;
    }

    public IEnumerator Attack()
    {
        GameObject _player = GameObject.FindGameObjectWithTag("Player");
        GameObject _tower = GameObject.FindGameObjectWithTag("Tower");
        yield return new WaitForSeconds(3.0f);
        if(FrontAttack(_player.transform))
        {
            _player.GetComponent<Player>().TakeDamage(_Attack, false);
        }
        else if(FrontAttack(_tower.transform))
        {
            _tower.GetComponent<Tower>().TakeDamage(_Attack/2.0f);
        }
        yield return new WaitForSeconds(0.5f);
        if(_Order != Order.Fight)
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

        if ((Vector3.Angle(ConeToTarget,Coneforward) < 90)&&(_Distance <= 100))
        {
            return true;
        }

        return false;
    }

    public IEnumerator DeathAnimation()
    {
        yield return new WaitForSeconds(1.0f);
        _Killed?.Invoke();
        yield return null;
    }

    public void UpdateAnimation()
    {
        throw new NotImplementedException();
    }
}
