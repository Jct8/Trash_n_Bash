using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Transform partToRotate;
    public GameObject bulletPrefeb;
    public Transform firePoint;

    private Transform _target;
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
    public float _shotTime;
    public bool isShooting = true;

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
        if (!isShooting)
        {
            return;
        }
        if(_target == null)
        {
            return;
        }
        Vector3 _direction = _target.position - transform.position;
        Quaternion _lookRotation = Quaternion.LookRotation(_direction);
        Vector3 _rotation = _lookRotation.eulerAngles;
        partToRotate.rotation = Quaternion.Euler(_rotation.x + 10.0f, _rotation.y, _rotation.z);

        if(_shotTime <= 0.0f)
        {
            Shoot();
            _shotTime = 1.0f / _attackRate;
        }

        _shotTime -= Time.deltaTime;
    }

    void Shoot()
    {
        GameObject _bulletGO = ServiceLocator.Get<ObjectPoolManager>().GetObjectFromPool(bulletPrefeb.name);
        _bulletGO.transform.position = firePoint.transform.position;
        _bulletGO.transform.rotation = firePoint.transform.rotation;
        _bulletGO.SetActive(true);
        _action = () => Recycle(_bulletGO);
        _bulletGO.GetComponent<Bullet>().Initialize(_target,_damage,_speed, _action);
        var rb = _bulletGO.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce(firePoint.up * _speed * 3.0f, ForceMode.Force);
    }

    public void TakeDamage(float dmg)
    {
        _FullHealth -= dmg;
        //Debug.Log("Taken damage: " + dmg);
        ServiceLocator.Get<UIManager>().UpdateTowerHealth(_FullHealth);
        if (_FullHealth <= 0.0f)
        {
            return;
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
