using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICharacterAction
{
    [SerializeField] private float health = 100.0f;
    [SerializeField] private float attack = 1.0f;
    [SerializeField] private float intimdateStunTime = 3.0f;
    [SerializeField] public float attackRange = 20.0f;
    [SerializeField] private float attackAngleRange = 45.0f;
    [SerializeField] private float poisonDamage = 10.0f;
    [SerializeField] private float poisonTotalTime = 3.0f;
    [SerializeField] private float poisonTickTime = 1.0f;
    [SerializeField] private float initialPoisonAttackDamage = 10.0f;
    [SerializeField] private float ultimateRange = 30.0f;
    [SerializeField] private float ultimateDamage = 25.0f;
    [SerializeField] private float ultimateTickDamage = 5.0f;
    [SerializeField] private float ultimateTotalTime = 3.0f;
    [SerializeField] private float ultimateTickTime = 1.0f;
    [SerializeField] private float ultimateChargeTime = 3.0f;
    [SerializeField] private float healedByItem = 5.0f;
    public const string DAMAGE_KEY = "Damage";
    public const string HEALTH_KEY = "Health";

    public GameObject Popup_Enemy;
    public GameObject Popup_Skunks;

    public AudioClip attackEffect;
    public AudioClip poisonEffect;
    public AudioClip UltimateEffect;
    AudioSource audioSource;

    private float _maxHealth = 100.0f;
    public float _ultimateCharge = 0.0f;
    private UIManager uiManager;
    public bool _ispoisoned = false;
    public float _poisonTickTime;
    public float _poisonDamage;
    public float _poisonCurrentTime;
    public float _poisonTotalTime = 5.0f;

    void Start()
    {
        _maxHealth = health;
        InvokeRepeating("IncrementUltCharge", 10.0f, ultimateChargeTime);
        uiManager = ServiceLocator.Get<UIManager>();
        audioSource = GetComponent<AudioSource>();
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

    public void SetPoisoned(float damage, float time, float total)
    {
        if (_ispoisoned)
            return;
        _poisonDamage = damage;
        _poisonTickTime = time;
        _poisonCurrentTime = Time.time;
        _poisonTotalTime = Time.time + total;
        _ispoisoned = true;
    }

    void Update()
    {
        if(!_ispoisoned)
        {
            return;
        }
        else
        {
            Tick();
        }
    }
    
    private void Tick()
    {
        if(_poisonCurrentTime < Time.time)
        {
            _poisonCurrentTime = Time.time + _poisonTickTime;
            TakeDamage(_poisonDamage, false, DamageType.Skunks);
        }
        if(_poisonTotalTime < Time.time)
        {
            _ispoisoned = false;
            return;
        }
    }

    public void TakeDamage(float damage, bool isHero, DamageType type)
    {
        health -= damage;
        switch(type)
        {
            case DamageType.Enemy:
                {
                    Popup_Enemy.GetComponent<TextMesh>().text = damage.ToString();
                    Instantiate(Popup_Enemy, transform.position, Camera.main.transform.rotation, transform);
                    Popup_Enemy.transform.Rotate(new Vector3(90.0f, 180.0f, 0.0f));
                    break;
                }
            case DamageType.Skunks:
                {
                    Popup_Skunks.GetComponent<TextMesh>().text = damage.ToString();
                    Instantiate(Popup_Skunks, transform.position, Camera.main.transform.rotation, transform);
                    Popup_Skunks.transform.Rotate(new Vector3(90.0f, 180.0f, 0.0f));
                    break;
                }
        }
        //Debug.Log("Player Took " + damage + " damage");
        ServiceLocator.Get<UIManager>().UpdatePlayerHealth(health);
    }

    public float UltimateCharge { get { return _ultimateCharge; } private set { } }
    public float Health { get { return health; } private set { } }

    public void IncrementUltCharge()
    {
        _ultimateCharge++;
        if (_ultimateCharge >= 100.0f)
        {
            _ultimateCharge = 100.0f;
        }
        uiManager.UpdateUltimatePercentage(_ultimateCharge);
    }

    public IEnumerator Attack()
    {
        ////Justin - TODO:Find a better method.

        List<string> ListOfEnemies = ServiceLocator.Get<ObjectPoolManager>().GetKeys();
        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        GameObject target = gameObject.GetComponent<PlayerController>().GetLockedOnTarget();

        if (target == null)
        {
            foreach (var enemy in ListOfEnemies)
            {
                List<GameObject> gameObjects = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects(enemy);
                foreach (var go in gameObjects)
                {
                    Vector3 direction = (go.transform.position - transform.position);
                    float distance = Vector3.Distance(transform.position, go.transform.position);
                    //float angle = Vector3.Angle(transform.forward, direction);
                    //if (Mathf.Abs(angle) < attackAngleRange && distance < attackRange)
                    //{
                    //    go.GetComponent<Enemy>().TakeDamage(attack, true);
                    //    gameObject.GetComponent<PlayerController>().SwitchAutoLock(go);
                    //}
                    if (distance < closestDistance && distance < attackRange)
                    {
                        closestDistance = distance;
                        closestEnemy = go;
                    }
                }
            }
        }
        else
            closestEnemy = target;

        if (closestEnemy && Vector3.Distance(transform.position, closestEnemy.transform.position) < attackRange)
        {
            closestEnemy?.GetComponent<Enemy>()?.TakeDamage(attack, true, DamageType.Normal);
            gameObject.GetComponent<PlayerController>().SwitchAutoLock(closestEnemy);
            audioSource.PlayOneShot(attackEffect, 0.75f);
        }
        yield return null;
        //target.GetComponent<Enemy>().TakeDamage(initialPoisonAttackDamage, true);
        //target.GetComponent<Enemy>().SetPoison(poisonDamage, poisonTickTime, poisonTotalTime);
    }

    public IEnumerator PoisonAttack()
    {
        //////Justin - TODO:Find a better method.
        //GameObject target = gameObject.GetComponent<PlayerController>().GetLockedOnTarget();

        //if (target)
        //{
        //    target.GetComponent<Enemy>().TakeDamage(initialPoisonAttackDamage, true);
        //    target.GetComponent<Enemy>().SetPoison(poisonDamage, poisonTickTime, poisonTotalTime);
        //    return;
        //}

        //List<string> ListOfEnemies = ServiceLocator.Get<ObjectPoolManager>().GetKeys();
        //foreach (var enemy in ListOfEnemies)
        //{
        //    List<GameObject> gameObjects = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects(enemy);
        //    foreach (var go in gameObjects)
        //    {
        //        Vector3 direction = (go.transform.position - transform.position);
        //        float distance = Vector2.Distance(transform.position, go.transform.position);
        //        float angle = Vector3.Angle(transform.forward, direction);
        //        if (Mathf.Abs(angle) < attackAngleRange && distance < attackRange)
        //        {
        //            go.GetComponent<Enemy>().TakeDamage(initialPoisonAttackDamage, true); // ERROR : Null Reference Excption
        //            go.GetComponent<Enemy>().SetPoison(poisonDamage, poisonTickTime, poisonTotalTime);
        //        }
        //    }
        //}

        List<string> ListOfEnemies = ServiceLocator.Get<ObjectPoolManager>().GetKeys();
        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        GameObject target = gameObject.GetComponent<PlayerController>().GetLockedOnTarget();

        if (target == null)
        {
            foreach (var enemy in ListOfEnemies)
            {
                List<GameObject> gameObjects = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects(enemy);
                foreach (var go in gameObjects)
                {
                    Vector3 direction = (go.transform.position - transform.position);
                    float distance = Vector3.Distance(transform.position, go.transform.position);
                    if (distance < closestDistance && distance < attackRange)
                    {
                        closestDistance = distance;
                        closestEnemy = go;
                    }
                }
            }
        }
        else
            closestEnemy = target;

        if (closestEnemy && Vector3.Distance(transform.position, closestEnemy.transform.position) < attackRange)
        {
            closestEnemy.GetComponent<Enemy>().TakeDamage(initialPoisonAttackDamage, true, DamageType.Poison);
            closestEnemy.GetComponent<Enemy>().SetPoison(poisonDamage, poisonTickTime, poisonTotalTime);
            gameObject.GetComponent<PlayerController>().SwitchAutoLock(closestEnemy);
            audioSource.PlayOneShot(poisonEffect, 0.5f);
        }
        yield return null;
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
                    go.GetComponent<Enemy>().TakeDamage(ultimateDamage, true, DamageType.Ultimate);
                    go.GetComponent<Enemy>().SetPoison(ultimateTickDamage, ultimateTickTime, ultimateTotalTime);
                    audioSource.PlayOneShot(UltimateEffect, 0.95f);
                }
            }
        }
    }

    public void IntimidateAttack( GameObject enemy)
    {
        if (enemy && Vector3.Distance(transform.position, enemy.transform.position) < attackRange)
        {
            enemy.GetComponent<Enemy>()._Order = Order.Stunned;
            enemy.GetComponent<Enemy>().stunTime = Time.time + intimdateStunTime;
        }
    }

    public GameObject DetectBarricade()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);

        foreach (var go in hitColliders)
        {
            Vector3 direction = (go.transform.position - transform.position);
            float distance = Vector2.Distance(transform.position, go.transform.position);
            float angle = Vector3.Angle(transform.forward, direction);
            if (Mathf.Abs(angle) < attackAngleRange && distance < attackRange && go.CompareTag("Barricade"))
            {
                return go.gameObject;
            }

        }
        return null;
    }

    private void OnTriggerEnter(Collider item)
    {
        if(item.gameObject.CompareTag("PickUp"))
        {
            item.gameObject.SetActive(false);
            GameObject tower = GameObject.FindGameObjectWithTag("Tower");
            tower.GetComponent<Tower>()._FullHealth += healedByItem;
            _maxHealth += healedByItem;
            
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
