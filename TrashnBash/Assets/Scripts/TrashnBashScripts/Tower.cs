using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    public GameObject signifierGO;
    public Image signifier;
    public Transform partToRotate;
    public GameObject bulletPrefeb;
    public Transform firePoint;
    public float radius = 2.5f;
    private Transform _target;
    private DataLoader dataLoader;
    private JsonDataSource towerData;

    public DamageType damageType = DamageType.Normal;
    public float fireDuration = 0.0f;

    private Action _action;
    [Header("Tower Status")]
    public string dataSourceId = "Tower";
    public string name;
    public float range = 2.0f;
    public float damage = 10.0f;
    public float speed = 5.0f;
    public float health = 100.0f;
    public float attackRate = 1.0f;
    public float fullHealth = 50.0f;
    public float shotTime;
    public bool isShooting = true;
    public string specificEnemy = "NONE";

    public AudioClip shotSound;
    AudioSource audioSource;

    [SerializeField]
    [Tooltip("Amount healed from Tower and  Trash cost to heal from Tower")]
    private float towerHealCostValue = 30.0f;
    [SerializeField]
    [Tooltip("Amount lost to Tower from healing the player")]
    private float towerLostCostValue = 15.0f;
    [SerializeField]
    [Tooltip("Cool Time for regaining Health from Tower")]

    public float minimumPlayerHealth = 30.0f;

    private float totalRegainCoolTime = 25.0f;
    private float currentRagainCoolTime = 0.0f;


    private void Awake()
    {
        dataLoader = ServiceLocator.Get<DataLoader>();
        if (App.Instance.hasGameLoaded)
        {
            towerData = dataLoader.GetDataSourceById(dataSourceId) as JsonDataSource;
            name = System.Convert.ToString(towerData.DataDictionary["Name"]);
        }

        audioSource = GetComponent<AudioSource>();
        fullHealth = health / 2.0f;
        InvokeRepeating("UpdateTarget", 0f, 0.1f);
    }

    private void Start()
    {
        if(signifier)
         signifier.fillAmount = 0;
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
        if (signifierGO)
            signifierGO.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
        CheckingClick();
        
        if(signifier)
        if (signifier.fillAmount <= 1)
        {
            signifier.fillAmount += 1 / totalRegainCoolTime * Time.deltaTime;
        }

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
            if(!_target.GetComponent<Enemy>().IsDead)
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
        _bulletGO.GetComponent<Bullet>().damageType = damageType;
        if(fireDuration != 0.0f)
        _bulletGO.GetComponent<Bullet>().fireTotalTime = fireDuration;
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

    public void CheckingClick()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Tower")
                {
                    restoring();
                }
            }
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
            string name = enemy.GetComponent<Enemy>().Name;
            if (_distanceToEnemy < _shortestDistance)
            {
                _shortestDistance = _distanceToEnemy;
                _nearestEnemy = enemy;
            }
            if ((name == specificEnemy) && _distanceToEnemy <= range)
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

    public void restoring()
    {
        UIManager uiManager = ServiceLocator.Get<UIManager>();
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        Player player = go.GetComponent<Player>();
        if (!uiManager || !player)
            return;
        if (player.health >= minimumPlayerHealth)
            return;
        if (fullHealth <= minimumPlayerHealth)
            return;

        if (signifier.fillAmount >= 1)
        {
            fullHealth -= towerLostCostValue;
            uiManager.UpdateTowerHealth(fullHealth);
            player.restoringHealth(towerHealCostValue);
            uiManager.UpdatePlayerHealth(player.health, player._maxHealth);
            signifier.fillAmount = 0;
        }
    }
}
