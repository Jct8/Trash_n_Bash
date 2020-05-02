using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICharacterAction
{
    #region Variables

    public float health = 100.0f;
    public float _maxHealth = 100.0f;
    [SerializeField] private float attack = 1.0f;
    [SerializeField] private float intimdateStunTime = 3.0f;
    [SerializeField] public float  attackRange = 20.0f;
    [SerializeField] private float poisonRange = 5.0f;
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
    [SerializeField] private float ultimateDelay = 2.0f;
    [SerializeField] private float healedByItem = 5.0f;
    public const string DAMAGE_KEY = "Damage";
    public const string HEALTH_KEY = "Health";

    [Header("Prefab")]
    public GameObject ultimateIndicator;
    public GameObject poisonIndicator;
    public GameObject popUp;
    public GameObject poisonAttack;
    public GameObject hitEffect;
    public GameObject Lighting;
    public GameObject LightingOnGround;
    public GameObject healingEffect;
    private UIManager _uiManager = null;
    //public GameObject restoreEffect;

    public AudioClip attackEffect;
    public AudioClip poisonEffect;
    public AudioClip UltimateEffect;
    public AudioClip LightingEffectSound;
    public AudioClip poisonedEffect;
    public AudioSource audioSource;


    public float _ultimateCharge = 0.0f;
    private float ultimateChargeStart = 0.0f;
    [Header("Poisoned player")]
    public bool _ispoisoned = false;
    public float _poisonTickTime;
    public float _poisonDamage;
    public float _poisonCurrentTime;
    public float _poisonTotalTime = 5.0f;
    public GameObject poison;
    private bool _isStoring = false;

    public float UltimateCharge { get { return _ultimateCharge; } private set { } }
    public float Health { get { return health; } private set { } }
    #endregion

    #region UnityFunctions

    void Start()
    {
        ResetPlayer();
        poisonAttack.SetActive(false);
        poisonIndicator.SetActive(false);
        poison.SetActive(_ispoisoned);
        _maxHealth = health;
        InvokeRepeating("IncrementUltCharge", 10.0f, ultimateChargeTime);
        audioSource = GetComponent<AudioSource>();
        ultimateChargeStart = _ultimateCharge;
        _uiManager = ServiceLocator.Get<UIManager>();
        _uiManager.UpdatePlayerHealth(health, _maxHealth);
        //attack = PlayerPrefs.GetFloat(DAMAGE_KEY, 20.0f);
        //health = PlayerPrefs.GetFloat(HEALTH_KEY, 100.0f);

        VariableLoader variableLoader = ServiceLocator.Get<VariableLoader>();
        if (variableLoader.useGoogleSheets)
        {
            _maxHealth = variableLoader.PlayerStats["HP"];
            healedByItem = variableLoader.PickUpStats["HealAmount"];

            attack = variableLoader.PlayerAbilties["Attack"]["Damage"];
            attackRange = variableLoader.PlayerAbilties["Attack"]["Range"];

            initialPoisonAttackDamage = variableLoader.PlayerAbilties["Poison"]["Damage"];
            poisonRange = variableLoader.PlayerAbilties["Poison"]["Range"];

            ultimateDamage = variableLoader.PlayerAbilties["Ultimate"]["Damage"];
            ultimateRange = variableLoader.PlayerAbilties["Ultimate"]["Range"];
        }
    }
    void Update()
    {
        if (!_ispoisoned)
        {
            return;
        }
        else
        {
            Tick();
        }
    }
    private void OnTriggerEnter(Collider item)
    {
        if (item.gameObject.CompareTag("PickUp"))
        {
            UIManager uiManager = ServiceLocator.Get<UIManager>();
            GameObject tower = ServiceLocator.Get<LevelManager>().towerInstance;
            item.gameObject.SetActive(false);
            tower.GetComponent<Tower>().fullHealth += healedByItem;

            if (tower.GetComponent<Tower>().fullHealth > 100.0f)
            {
                tower.GetComponent<Tower>().fullHealth = 100.0f;
            }
            health += healedByItem;
            if (_maxHealth > 100.0f)
            {
                _maxHealth = 100.0f;
            }
            uiManager.UpdateTowerHealth(tower.GetComponent<Tower>().fullHealth);
            uiManager.UpdatePlayerHealth(health, _maxHealth);
        }
    }

    #endregion

    #region Initialization
    public void Initialize(float dmg, float hp)
    {
        poison.SetActive(_ispoisoned);
        attack = dmg;
        health = hp;
        _maxHealth = health;
        ultimateChargeStart = _ultimateCharge;
    }

    public void ResetPlayer()
    {
        poison.SetActive(false);
        _maxHealth = health;
        ultimateChargeStart = _ultimateCharge;
    }

    //public void SaveData(float dmg, float hp)
    //{
    //    PlayerPrefs.SetFloat(DAMAGE_KEY, dmg);
    //    PlayerPrefs.SetFloat(HEALTH_KEY, hp);
    //}
    #endregion

    #region Take Damage

    public void SetPoisoned(float damage, float time, float total)
    {
        if (_ispoisoned)
            return;
        _poisonDamage = damage;
        _poisonTickTime = time;
        _poisonCurrentTime = Time.time;
        _poisonTotalTime = Time.time + total;
        _ispoisoned = true;
        poison.SetActive(_ispoisoned);
    }

    private void Tick()
    {
        if (_poisonCurrentTime < Time.time)
        {
            _poisonCurrentTime = Time.time + _poisonTickTime;
            TakeDamage(_poisonDamage, false, DamageType.Skunks);
        }
        if (_poisonTotalTime < Time.time)
        {
            _ispoisoned = false;
            poison.SetActive(_ispoisoned);
            return;
        }
    }

    public void TakeDamage(float damage, bool isHero, DamageType type)
    {
        health -= damage;
        _uiManager.UpdatePlayerHealth(health, _maxHealth);
        popUp.GetComponent<TextMesh>().text = damage.ToString();
        switch (type)
        {
            case DamageType.Enemy:
                {
                    popUp.GetComponent<TextMesh>().color = new Color(1.0f, 0.0f, 1.0f);
                    GameObject hit = Instantiate(hitEffect, transform.position, Quaternion.identity) as GameObject;
                    break;
                }
            case DamageType.Skunks:
                {
                    popUp.GetComponent<TextMesh>().color = new Color(0.0f, 1.0f, 0.0f);
                    break;
                }
        }
        //popUp.transform.Rotate(new Vector3(90.0f, 180.0f, 0.0f));
        Instantiate(popUp, transform.position, Camera.main.transform.rotation, transform);
        //Debug.Log("Player Took " + damage + " damage");
        ServiceLocator.Get<UIManager>().UpdatePlayerHealth(health, _maxHealth);
    }

    #endregion

    #region Attacks

    public void IncrementUltCharge()
    {
        _ultimateCharge++;
        if (_ultimateCharge >= 100.0f)
        {
            _ultimateCharge = 100.0f;
        }

        UIManager uiManager = ServiceLocator.Get<UIManager>();
        uiManager.UpdateUltimatePercentage(_ultimateCharge);
    }

    public IEnumerator Attack()
    {
        ////Justin - TODO:Find a better method.

        List<string> ListOfEnemies = ServiceLocator.Get<ObjectPoolManager>().GetKeys();
        //float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        GameObject target = gameObject.GetComponent<PlayerController>().GetLockedOnTarget();

        closestEnemy = target;

        if (closestEnemy && Vector3.Distance(transform.position, closestEnemy.transform.position) < attackRange)
        {
            closestEnemy?.GetComponent<Enemy>()?.TakeDamage(attack, true, DamageType.Normal);
            gameObject.GetComponent<PlayerController>().SwitchAutoLock(closestEnemy);
            audioSource.PlayOneShot(attackEffect, 0.75f);
        }
        yield return null;
    }

    public IEnumerator PoisonAttack()
    {
        //////Justin - TODO:Find a better method.


        //float closestDistance = Mathf.Infinity;
        //GameObject closestEnemy = null;

        //GameObject target = gameObject.GetComponent<PlayerController>().GetLockedOnTarget();

        //if (target == null)
        //{
        //    foreach (var enemy in ListOfEnemies)
        //    {
        //        List<GameObject> gameObjects = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects(enemy);
        //        foreach (var go in gameObjects)
        //        {
        //            Vector3 direction = (go.transform.position - transform.position);
        //            float distance = Vector3.Distance(transform.position, go.transform.position);
        //            if (distance < closestDistance && distance < attackRange)
        //            {
        //                closestDistance = distance;
        //                closestEnemy = go;
        //            }
        //        }
        //    }
        //}
        //else
        //    closestEnemy = target;

        //if (target == null)
        //{
        //    foreach (var enemy in ListOfEnemies)
        //    {
        //        List<GameObject> gameObjects = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects(enemy);
        //        foreach (var go in gameObjects)
        //        {
        //            Vector3 direction = (go.transform.position - transform.position);
        //            float distance = Vector3.Distance(transform.position, go.transform.position);
        //            //float angle = Vector3.Angle(transform.forward, direction);
        //            //if (Mathf.Abs(angle) < attackAngleRange && distance < attackRange)
        //            //{
        //            //    go.GetComponent<Enemy>().TakeDamage(attack, true);
        //            //    gameObject.GetComponent<PlayerController>().SwitchAutoLock(go);
        //            //}
        //            if (distance < closestDistance && distance < attackRange)
        //            {
        //                closestDistance = distance;
        //                closestEnemy = go;
        //            }
        //        }
        //    }
        //}
        //else
        //{
        //   closestEnemy = target;
        //}
        poisonIndicator.GetComponent<Transform>().localScale = new Vector3(poisonRange + 1.0f, 0.007460861f, poisonRange + 1.0f);
        poisonIndicator.SetActive(true);
        List<string> ListOfEnemies = ServiceLocator.Get<ObjectPoolManager>().GetKeys();

        foreach (var enemy in ListOfEnemies)
        {
            List<GameObject> gameObjects = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects(enemy);
            foreach (var go in gameObjects)
            {
                float distance = Vector2.Distance(transform.position, go.transform.position);
                if (distance < poisonRange)
                {
                    go.GetComponent<Enemy>()?.TakeDamage(initialPoisonAttackDamage, true, DamageType.Poison);
                    go.GetComponent<Enemy>()?.SetPoison(poisonDamage, poisonTickTime, poisonTotalTime);
                }
            }
        }
        audioSource.PlayOneShot(poisonEffect, 0.5f);
        poisonAttack.SetActive(true);

        yield return new WaitForSeconds(1.0f);
        poisonIndicator.SetActive(!poisonIndicator.activeSelf);

        audioSource.PlayOneShot(poisonedEffect, 0.2f);
        poisonAttack.SetActive(false);

        yield return null;
    }

    public IEnumerator UltimateAttack()
    {
        if (_ultimateCharge != 100.0f)
            yield break;
        ////Justin - TODO:Find a better method.
        ultimateIndicator.GetComponent<Transform>().localScale = new Vector3(ultimateRange, 0.007460861f, ultimateRange);
        ultimateIndicator.SetActive(true);
        yield return new WaitForSeconds(ultimateDelay);

        ultimateIndicator.SetActive(false);
        _ultimateCharge = 0.0f;
        List<string> ListOfEnemies = ServiceLocator.Get<ObjectPoolManager>().GetKeys();
        foreach (var enemy in ListOfEnemies)
        {
            List<GameObject> gameObjects = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects(enemy);
            foreach (var go in gameObjects)
            {
                float distance = Vector2.Distance(transform.position, go.transform.position);
                if (distance < ultimateRange*0.5f)
                {
                    go.GetComponent<Enemy>()?.TakeDamage(ultimateDamage, true, DamageType.Ultimate);
                    go.GetComponent<Enemy>()?.SetPoison(ultimateTickDamage, ultimateTickTime, ultimateTotalTime);
                }
            }
        }
        audioSource.PlayOneShot(UltimateEffect, 0.95f);
    }

    public void IntimidateAttack(GameObject enemy)
    {
        if (enemy && Vector3.Distance(transform.position, enemy.transform.position) < attackRange)
        {
            Instantiate(Lighting, enemy.transform.position, Quaternion.identity);
            Instantiate(LightingOnGround, gameObject.transform.position, Quaternion.identity);
            audioSource.PlayOneShot(LightingEffectSound, 0.5f);
            enemy.GetComponent<Enemy>()._Order = Order.Stunned;
            enemy.GetComponent<Enemy>().stunTime = Time.time + intimdateStunTime;
        }
    }
    #endregion

    #region Checks and Utility Functions

    public GameObject DetectBarricadeSpawner()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);

        foreach (var go in hitColliders)
        {
            Vector3 direction = (go.transform.position - transform.position);
            float distance = Vector2.Distance(transform.position, go.transform.position);
            float angle = Vector3.Angle(transform.forward, direction);
            if (distance < attackRange && go.CompareTag("BarricadeSpawner"))
            {
                return go.GetComponent<BarricadeSpawner>().GetBarricade();
            }
        }
        return null;
    }

    public GameObject DetectBarricade()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);

        foreach (var go in hitColliders)
        {
            Vector3 direction = (go.transform.position - transform.position);
            float distance = Vector2.Distance(transform.position, go.transform.position);
            float angle = Vector3.Angle(transform.forward, direction);
            if (distance < attackRange && go.CompareTag("Barricade"))
            {
                return go.gameObject;
            }
        }
        return null;
    }

    public void restoringHealth(float value)
    {
        StartCoroutine(DisplayHealingEffect());
        health += value;
    }

    private IEnumerator DisplayHealingEffect()
    {
        if(!_isStoring)
        {
            _isStoring = true;
            GameObject heal = Instantiate(healingEffect, gameObject.transform.position, Quaternion.identity) as GameObject;
            yield return new WaitForSeconds(2.0f);
            _isStoring = false;
        }
        else
        {
            yield return null;
        }
 
    }
    #endregion

    #region Animations

    public void UpdateAnimation()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator DeathAnimation()
    {
        throw new System.NotImplementedException();
    }

    #endregion

    public void FullUltimate()
    {
        _ultimateCharge = 100.0f;
    }
}
