using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICharacterAction
{
    [SerializeField] private float health = 100.0f;
    [SerializeField] private float attack = 1.0f;
    [SerializeField] private float attackRange = 20.0f;
    [SerializeField] private float attackAngleRange = 45.0f;
    [SerializeField] private float poisonDamage = 10.0f;
    [SerializeField] private float poisonTotalTime = 3.0f;
    [SerializeField] private float poisonTickTime = 1.0f;
    [SerializeField] private float initialPoisonAttackDamage = 10.0f;
    [SerializeField] private float ultimateRange = 30.0f;
    [SerializeField] private float ultimateDamage = 25.0f ;
    [SerializeField] private float ultimateTickDamage = 5.0f;
    [SerializeField] private float ultimateTotalTime = 3.0f;
    [SerializeField] private float ultimateTickTime = 1.0f;
    [SerializeField] private float ultimateChargeTime = 3.0f;

    public const string DAMAGE_KEY = "Damage";
    public const string HEALTH_KEY = "Health";

    private float _maxHealth = 100.0f;
    public float _ultimateCharge = 0.0f;
    private UIManager uiManager;

    void Start()
    {
        _maxHealth = health;
        InvokeRepeating("IncrementUltCharge", 10.0f, ultimateChargeTime);
        uiManager = ServiceLocator.Get<UIManager>();
        //attack = PlayerPrefs.GetFloat(DAMAGE_KEY, 20.0f);
        //health = PlayerPrefs.GetFloat(HEALTH_KEY, 100.0f);
    }

    public void Initialize(float dmg, float hp)
    {
        attack = dmg;
        health = hp;
        _maxHealth = health;
    }

    //public void SaveData(float dmg, float hp)
    //{
    //    PlayerPrefs.SetFloat(DAMAGE_KEY, dmg);
    //    PlayerPrefs.SetFloat(HEALTH_KEY, hp);
    //}

    public void TakeDamage(float damage, bool isHero)
    {
        health -= damage;
        //Debug.Log("Player Took " + damage + " damage");
        ServiceLocator.Get<UIManager>().UpdatePlayerHealth(health);
    }

    public float UltimateCharge { get { return _ultimateCharge; } private set { } }
    public float Health { get { return health; } private set { } }

    public void IncrementUltCharge()
    {
        _ultimateCharge++;
        if (_ultimateCharge>=100.0f)
        {
            _ultimateCharge = 100.0f;
        }
        uiManager.UpdateUltimatePercentage(_ultimateCharge);
    }

    public IEnumerator Attack()
    {
        ////Justin - TODO:Find a better method.
        List<string> ListOfEnemies = ServiceLocator.Get<ObjectPoolManager>().GetKeys();
        foreach (var enemy in ListOfEnemies)
        {
            List<GameObject> gameObjects = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects(enemy);
            foreach (var go in gameObjects)
            {
                Vector3 direction = (go.transform.position - transform.position);
                float distance = Vector2.Distance(transform.position, go.transform.position);
                float angle = Vector3.Angle(transform.forward, direction);
                if (Mathf.Abs(angle) < attackAngleRange && distance < attackRange)
                {
                    go.GetComponent<Enemy>().TakeDamage(attack, true);
                }
            }
        }
        yield return null;
    }

    public void PoisonAttack()
    {
        ////Justin - TODO:Find a better method.
        List<string> ListOfEnemies = ServiceLocator.Get<ObjectPoolManager>().GetKeys();
        foreach (var enemy in ListOfEnemies)
        {
            List<GameObject> gameObjects = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects(enemy);
            foreach (var go in gameObjects)
            {
                Vector3 direction = (go.transform.position - transform.position);
                float distance = Vector2.Distance(transform.position, go.transform.position);
                float angle = Vector3.Angle(transform.forward, direction);
                if (Mathf.Abs(angle) < attackAngleRange && distance < attackRange)
                {
                    go.GetComponent<Enemy>().TakeDamage(initialPoisonAttackDamage,true); // ERROR : Null Reference Excption
                    go.GetComponent<Enemy>().SetPoison(poisonDamage, poisonTickTime, poisonTotalTime);
                }
            }
        }
    }

    public void UltimateAttack()
    {
        if (_ultimateCharge != 100.0f)
            return;
        ////Justin - TODO:Find a better method.
        _ultimateCharge = 0.0f;
        List<string> ListOfEnemies = ServiceLocator.Get<ObjectPoolManager>().GetKeys();
        foreach (var enemy in ListOfEnemies)
        {
            List<GameObject> gameObjects = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects(enemy);
            foreach (var go in gameObjects)
            {
                float distance = Vector2.Distance(transform.position, go.transform.position);
                if (distance < ultimateRange)
                {
                    go.GetComponent<Enemy>().TakeDamage(ultimateDamage, true);
                    go.GetComponent<Enemy>().SetPoison(ultimateTickDamage, ultimateTickTime, ultimateTotalTime);
                }
            }
        }
    }

    public void UpdateAnimation()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator DeathAnimation()
    {
        throw new System.NotImplementedException();
    }
}
