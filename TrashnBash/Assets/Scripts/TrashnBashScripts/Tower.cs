using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Transform partToRotate;
    public GameObject bulletPrefeb;
    public Transform firePoint;
    public float radius = 2.5f;
    private Transform _target;
    private DataLoader dataLoader;
    private JsonDataSource towerData;

    private Action _action;

    public string dataSourceId = "Tower";
    public string name;
    public float range;
    public float damage;
    public float speed;
    public float health;
    public float attackRate;
    public float fullHealth;
    public float shotTime;
    public bool isShooting = true;

    public AudioClip shotSound;
    AudioSource audioSource;

    private void Awake()
    {
        dataLoader = ServiceLocator.Get<DataLoader>();
        towerData = dataLoader.GetDataSourceById(dataSourceId) as JsonDataSource;
        name = System.Convert.ToString(towerData.DataDictionary["Name"]);
        damage = System.Convert.ToSingle(towerData.DataDictionary["Damage"]);
        speed = System.Convert.ToSingle(towerData.DataDictionary["Speed"]);
        health = System.Convert.ToSingle(towerData.DataDictionary["Health"]);
        attackRate = System.Convert.ToSingle(towerData.DataDictionary["AttackRate"]);
        range = System.Convert.ToSingle(towerData.DataDictionary["Range"]);
        audioSource = GetComponent<AudioSource>();
        fullHealth = health / 2.0f;
        InvokeRepeating("UpdateTarget", 0f, 0.1f);
    }
    public void Initialize(float dmg, float s, float h, float ar, float r)
    {
        damage = dmg;
        speed = s;
        health = h;
        attackRate = ar;
        range = r;
        fullHealth = health;
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

        if(shotTime <= 0.0f)
        {
            if(!_target.GetComponent<Enemy>().Dead)
            {
                Shoot();
                shotTime = 1.0f / attackRate;
            }

        }

        shotTime -= Time.deltaTime;
    }

    void Shoot()
    {
        GameObject _bulletGO = ServiceLocator.Get<ObjectPoolManager>().GetObjectFromPool(bulletPrefeb.name);
        _bulletGO.transform.position = firePoint.transform.position;
        _bulletGO.transform.rotation = firePoint.transform.rotation;
        _bulletGO.SetActive(true);
        _action = () => Recycle(_bulletGO);
        _bulletGO.GetComponent<Bullet>().Initialize(_target,damage,speed, _action);
        var rb = _bulletGO.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce(firePoint.up * speed * 3.0f, ForceMode.Force);
        audioSource.PlayOneShot(shotSound, 1.0f);
    }

    public void TakeDamage(float dmg)
    {
        UIManager uiManager = ServiceLocator.Get<UIManager>();
        fullHealth -= dmg;
        //Debug.Log("Taken damage: " + dmg);
        uiManager.UpdateTowerHealth(fullHealth);
        if (fullHealth <= 0.0f)
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
        if(_nearestEnemy != null && _shortestDistance <= range)
        {
            _target = _nearestEnemy.transform;
        }
        else
        {
            _target = null;
        }
    }

    public float Getradius()
    {
        return radius * radius;
    }
}
