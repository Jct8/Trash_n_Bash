using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private Transform _target;
    public Transform _partToRotate;
    public GameObject _bulletPrefeb;
    public Transform _firePoint;

    private DataLoader _dataLoader;
    private JsonDataSource _towerData;

    private Action _action;

    public string _dataSourceId = "Tower";
    public string _name;
    public float _range;
    public float _damage;
    public float _speed;
    public float _health;
    public float _attackRate;

    public float _FullHealth;
    private float _shotTime;

    private void Awake()
    {
        _dataLoader = ServiceLocator.Get<DataLoader>();
        _towerData = _dataLoader.GetDataSourceById(_dataSourceId) as JsonDataSource;
        _name = System.Convert.ToString(_towerData.DataDictionary["Name"]);
        _damage = System.Convert.ToSingle(_towerData.DataDictionary["Damage"]);
        _speed = System.Convert.ToSingle(_towerData.DataDictionary["Speed"]);
        _health = System.Convert.ToSingle(_towerData.DataDictionary["Health"]);
        _attackRate = System.Convert.ToSingle(_towerData.DataDictionary["AttackRate"]);
        _range = System.Convert.ToSingle(_towerData.DataDictionary["Range"]);
        _FullHealth = _health;
        InvokeRepeating("UpdateTarget", 0f, 0.1f);
    }
    public void Initialize(float damage, float speed, float health, float attackR, float range)
    {
        _damage = damage;
        _speed = speed;
        _health = health;
        _attackRate = attackR;
        _range = range;
        _FullHealth = _health;
    }

    void Update()
    {
        return;
        if(_target == null)
        {
            return;
        }

        Vector3 _direction = _target.position - transform.position;
        Quaternion _lookRotation = Quaternion.LookRotation(_direction);
        Vector3 _rotation = _lookRotation.eulerAngles;
        _partToRotate.rotation = Quaternion.Euler(0.0f, _rotation.y, 0.0f);

        if(_shotTime <= 0.0f)
        {
            Shoot();
            _shotTime = 1.0f / _attackRate;
        }

        _shotTime -= Time.deltaTime;
    }

    void Shoot()
    {
        GameObject _bulletGO = ServiceLocator.Get<ObjectPoolManager>().GetObjectFromPool(_bulletPrefeb.name);
        _bulletGO.transform.position = _firePoint.transform.position;
        _bulletGO.transform.rotation = _firePoint.transform.rotation;
        _bulletGO.SetActive(true);
        _action = () => Recycle(_bulletGO);
        _bulletGO.GetComponent<Bullet>().Initialize(_target,_damage,_speed, _action);
        var rb = _bulletGO.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce(_firePoint.up * _speed*5.0f, ForceMode.Force);
    }

    public void TakeDamage(float dmg)
    {
        _FullHealth -= dmg;
        if(_FullHealth <= 0.0f)
        {
            Destroy(this);
        }
    }

    public void Recycle(GameObject obj)
    {
        ServiceLocator.Get<ObjectPoolManager>().RecycleObject(obj);
    }

    void UpdateTarget()
    {
        GameObject[] _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float _shortestDistance = Mathf.Infinity;
        GameObject _nearestEnemy = null;
        foreach(GameObject enemy in _enemies)
        {
            float _distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if(_distanceToEnemy < _shortestDistance)
            {
                _shortestDistance = _distanceToEnemy;
                _nearestEnemy = enemy;
            }
        }
        if(_nearestEnemy != null && _shortestDistance <= _range)
        {
            _target = _nearestEnemy.transform;
        }
        else
        {
            _target = null;
        }
    }
}
