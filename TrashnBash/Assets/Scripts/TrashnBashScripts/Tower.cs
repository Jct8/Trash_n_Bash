using SheetCodes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    public GameObject signifierGO;
    public Transform partToRotate;
    public GameObject bulletPrefeb;
    public Transform firePoint;
    public float radius = 2.5f;
    private GameObject _nearestEnemy;
    private Transform _target;
    private DataLoader dataLoader;
    private JsonDataSource towerData;
    private UIManager uiManager;

    public DamageType damageType = DamageType.Normal;
    public float fireDuration = 0.0f;

    private Action _action;
    [Header("Tower Status")]
    public string dataSourceId = "Tower";
    public float range = 2.0f;
    [HideInInspector]
    public float rangeBeforeUpgrade = 2.0f; //Used for upgrades
    [HideInInspector]
    public float rangeAfterUpgrade = 2.0f;//Used for upgrades
    public float damage = 10.0f;
    public float bulletSpeed = 5.0f;
    public float MaxHealth = 100.0f;
    public float attackRate = 1.0f;
    public float fullHealth = 50.0f;
    public float shotTime;
    public bool isShooting = true;
    public string specificEnemy = "No Target";

    public AudioClip shotSound;
    private AudioSource audioSource;
    private AudioManager audioManager;

    [SerializeField]
    [Tooltip("Amount healed from Tower and  Trash cost to heal from Tower")]
    public float towerHealCostValue = 30.0f;
    [HideInInspector]
    public float towerHealBeforeUpgrade = 30.0f;
    [HideInInspector]
    public float towerHealAfterUpgrade = 30.0f;
    [SerializeField]
    [Tooltip("Amount lost to Tower from healing the player")]
    private float towerLostCostValue = 15.0f;
    [SerializeField]
    [Tooltip("Cool Time for regaining Health from Tower")]
    private float totalRegainCoolTime = 25.0f;
    [SerializeField]
    [Tooltip("Activate regaining health for Player health")]
    private float minimumPlayerHealth = 70.0f;
    [SerializeField]
    [Tooltip("Inactivate regaining health if the Tower has low Health")]
    private float minimumTowerHealth = 20.0f;


    private void Awake()
    {
        dataLoader = ServiceLocator.Get<DataLoader>();
        if (App.Instance.hasGameLoaded)
        {
            towerData = dataLoader.GetDataSourceById(dataSourceId) as JsonDataSource;
            name = System.Convert.ToString(towerData.DataDictionary["Name"]);
        }
        audioSource = GetComponent<AudioSource>();
        audioManager = ServiceLocator.Get<AudioManager>();
        fullHealth = MaxHealth / 2.0f;
        InvokeRepeating("UpdateTarget", 0f, 0.1f);

    }

    private void Start()
    {
        uiManager = ServiceLocator.Get<UIManager>();
        VariableLoader variableLoader = ServiceLocator.Get<VariableLoader>();
        if (variableLoader.useGoogleSheets)
        {
            MaxHealth = variableLoader.TowerStats["Health"];
            towerHealCostValue = variableLoader.TowerStats["PlayerHeal"];
            towerLostCostValue = variableLoader.TowerStats["TrashCost"];

            fullHealth = MaxHealth;
        }
        ///////////  Upgrades - Improved healing  ///////////
        int level = ServiceLocator.Get<GameManager>().upgradeLevelsDictionary[UpgradeMenu.Upgrade.ImprovedHealing];
        UpgradesIdentifier upgradesIdentifier = ModelManager.UpgradesModel.GetUpgradeEnum(UpgradeMenu.Upgrade.ImprovedHealing, level);
        towerHealBeforeUpgrade = towerHealCostValue;
        if (level >= 1)
        {
            towerHealCostValue += ModelManager.UpgradesModel.GetRecord(upgradesIdentifier).ModifierValue;
            towerHealAfterUpgrade = towerHealCostValue;
        }
        fullHealth = ServiceLocator.Get<GameManager>()._houseHP;

        ///////////  Upgrades - Long Ranged Upgrade  ///////////
        int rangedLevel = ServiceLocator.Get<GameManager>().upgradeLevelsDictionary[UpgradeMenu.Upgrade.Ranged] - 1;
        upgradesIdentifier = ModelManager.UpgradesModel.GetUpgradeEnum(UpgradeMenu.Upgrade.Ranged, rangedLevel + 1);
        rangeBeforeUpgrade = range;
        if (rangedLevel >= 0 && gameObject.CompareTag("Tower"))
        {
            range += ModelManager.UpgradesModel.GetRecord(upgradesIdentifier).ModifierValue;// upgradeStats.towerRange[rangedLevel];
            rangeAfterUpgrade = range;
        }

    }

    public void Initialize(float dmg, float s, float h, float ar, float r)
    {
        damage = dmg;
        bulletSpeed = s;
        MaxHealth = h;
        attackRate = ar;
        range = r;
        fullHealth = MaxHealth;
    }

    void Update()
    {
        Player player = ServiceLocator.Get<LevelManager>().playerInstance.GetComponent<Player>();

        if (signifierGO)
        {
            if (player.GetComponent<Player>()?.health > minimumPlayerHealth)
            {
                signifierGO.SetActive(false);
            }
            else
            {
                signifierGO.SetActive(true);
            }
        }

        if (signifierGO)
        {
            signifierGO.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
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
                if(_target.GetComponent<Collider>().enabled)
                    Shoot(_target.GetComponent<Enemy>());
                shotTime = 1.0f / attackRate;
            }

        }

        shotTime -= Time.deltaTime;
    }

    void Shoot(Enemy target)
    {
        GameObject _bulletGO = ServiceLocator.Get<ObjectPoolManager>().GetObjectFromPool(bulletPrefeb.name);
        _bulletGO.transform.position = firePoint.transform.position;
        _bulletGO.transform.rotation = firePoint.transform.rotation;
        _bulletGO.SetActive(true);
        _action = () => Recycle(_bulletGO);
        _bulletGO.GetComponent<Bullet>().Initialize(target.transform, damage, bulletSpeed, _action);
        _bulletGO.GetComponent<Bullet>().SetBulletType(damageType);
        if(fireDuration != 0.0f)
            _bulletGO.GetComponent<Bullet>().fireTotalTime = fireDuration;
        audioManager.PlaySfx(shotSound);
    }

    public void TakeDamage(float dmg)
    {
        fullHealth -= dmg;
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

    public void restoring()
    {
        Player player = ServiceLocator.Get<LevelManager>().playerInstance.GetComponent<Player>();

        if (!player)
            return;
        if (fullHealth < minimumTowerHealth)
            return;

        // Lose Tower's health
        fullHealth -= towerLostCostValue;
        uiManager.UpdateTowerHealth(fullHealth);

        // Heal Player's health
        player.restoringHealth(towerHealCostValue);
        uiManager.UpdatePlayerHealth(player.health, player._maxHealth);
    }

    public void HealTower(float value)
    {
        fullHealth += value;
        uiManager.UpdateTowerHealth(fullHealth);
    }

}
