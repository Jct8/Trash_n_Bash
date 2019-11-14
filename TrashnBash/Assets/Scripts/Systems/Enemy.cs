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

    public string _DataSource;
    public string _Name;
    public float _Attack;
    public float _Speed;
    public float _Health;
    public float _Money;
    
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
        _Name = System.Convert.ToString(_EnemyData.DataDictionary["Name"]);
        _Attack = System.Convert.ToSingle(_EnemyData.DataDictionary["Attack"]);
        _Speed = System.Convert.ToSingle(_EnemyData.DataDictionary["Speed"]);
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
            _Agent.SetDestination(_Desination.position);
            if (Vector3.Distance(transform.position, _Desination.position) < 2.0f)
            {
                if(_CurrentWayPoint == 2)
                {
                    //Attack(_Attack);
                    if(_Detect == Detect.Attack)
                    {
                        _Agent.SetDestination(_Player.transform.position);
                    }
                }
                else
                {
                    _CurrentWayPoint++;
                }
            }
            Detection();
            _Rigid.velocity = Vector3.zero;
        }
        else
        {
            _Rigid.velocity = Vector3.zero;
        }
    }

    public void Detection()
    {
        if ((_Detect != Detect.Attack || _Detect != Detect.Detected) && 
            Vector3.Distance(_Path.WayPoints[2].transform.position, transform.position) < 4.0f)
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
        _CurrentWayPoint = 0;
        _FullHealth = _Health;
        _Rigid.velocity = Vector3.zero;
    }

    public void TakeDamage(float Dmg, bool isHero)
    {
        _FullHealth -= Dmg;

        if (_Detect == Detect.Detected && isHero == true)
            _Detect = Detect.Attack;
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
            TakeDamage(poisonDamage);
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

    public void Attack(float Dmg)
    {
        throw new NotImplementedException();
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
