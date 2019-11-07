using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, ICharacterAction
{
    public Action _Killed;
    private DataLoader _DataLoader;
    private JsonDataSource _EnemyData;
    private NavMeshAgent _Agent;
    private WayPointManager.Path _Path;

    public float _FullHealth;
    public int _CurrentWayPoint = 0;
    private bool _IsDead = false;

    public string _DataSource;
    public string _Name;
    public float _Attack;
    public float _Speed;
    public float _Health;
    public float _Money;

    public void Initialize(WayPointManager.Path path, Action Recycle)
    {
        _Path = path;
        _Killed += Recycle;
        _DataLoader = ServiceLocator.Get<DataLoader>();
        _EnemyData = _DataLoader.GetDataSourceById(_DataSource) as JsonDataSource;
        _IsDead = false;
        _Agent = GetComponent<NavMeshAgent>();

        _Name = System.Convert.ToString(_EnemyData.DataDictionary["Name"]);
        _Attack = System.Convert.ToSingle(_EnemyData.DataDictionary["Attack"]);
        _Speed = System.Convert.ToSingle(_EnemyData.DataDictionary["Speed"]);
        _Health = System.Convert.ToSingle(_EnemyData.DataDictionary["Health"]);
        _Money = System.Convert.ToSingle(_EnemyData.DataDictionary["Money"]);

        _FullHealth = _Health;
    }
    
    void Update()
    {
        if(!_IsDead)
        {
            //UpdateAnimation();
            Transform _Desination = _Path.WayPoints[_CurrentWayPoint];
            _Agent.SetDestination(_Desination.position);
            if (Vector3.Distance(transform.position, _Desination.position) < 2.0f)
            {
                if(_CurrentWayPoint == 2)
                {
                    //Attack(_Attack);
                }
                else
                {
                    _CurrentWayPoint++;
                }
            }
        }
    }

    public void Alive()
    {
        _IsDead = false;
        _CurrentWayPoint = 0;
        _FullHealth = _Health;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void TakeDamage(float Dmg)
    {
        _FullHealth -= Dmg;
        if(_FullHealth <= 0.0f)
        {
            _IsDead = true;
            StartCoroutine("DeathAnimation");
        }
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
