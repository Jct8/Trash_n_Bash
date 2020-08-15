using SheetCodes;
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
    [SerializeField] public float attackRange = 20.0f;
    [SerializeField] private float poisonRange = 5.0f;
    [SerializeField] private float stunRange = 3.0f;
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

    [Header("Shaking camera for Ultimate")]
    [SerializeField] private float _duration = 2.0f;
    [SerializeField] private float _shakeAmount = 0.7f;
    [SerializeField] private float _decrFactor = 1.0f;

    [Header("Prefab")]
    public GameObject ultimateIndicator;
    public GameObject poisonIndicator;
    public GameObject stunIndicator;
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
    public AudioManager audioManager;
    public ParticleSystem basicHitParticle;
    public ParticleSystem stunParticle;

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
    public bool isAlive = true;

    public float UltimateCharge { get { return _ultimateCharge; } private set { } }
    public float Health { get { return health; } private set { } }

    ICharacterSound characterSound;
    private Animator animator;

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
        audioManager = ServiceLocator.Get<AudioManager>();

        ultimateChargeStart = _ultimateCharge;
        _uiManager = ServiceLocator.Get<UIManager>();
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

            stunRange = variableLoader.PlayerAbilties["Stun"]["Range"];

            ultimateDamage = variableLoader.PlayerAbilties["Ultimate"]["Damage"];
            ultimateRange = variableLoader.PlayerAbilties["Ultimate"]["Range"];
        }
        ///////////  Upgrades - Improved Player HP  ///////////
        int level = ServiceLocator.Get<GameManager>().upgradeLevelsDictionary[UpgradeMenu.Upgrade.ImprovedPlayerHP];
        UpgradesIdentifier upgradesIdentifier = ModelManager.UpgradesModel.GetUpgradeEnum(UpgradeMenu.Upgrade.ImprovedPlayerHP, level);
        if (level >= 1)
            _maxHealth += ModelManager.UpgradesModel.GetRecord(upgradesIdentifier).ModifierValue;

        health = _maxHealth;
        _uiManager.UpdatePlayerHealth(health, _maxHealth);
        animator = GetComponent<Animator>();
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
            StartCoroutine(characterSound.BasicSound(5));
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
        characterSound = GetComponent<ICharacterSound>();
    }

    public void ResetPlayer()
    {
        if (animator)
            animator.SetBool("Dead", false);
        isAlive = true;
        poison.SetActive(false);
        _maxHealth = health;
        ultimateChargeStart = _ultimateCharge;
        characterSound = GetComponent<ICharacterSound>();
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
        StartCoroutine(characterSound.BasicSound(2));
        poison.SetActive(_ispoisoned);
    }

    private void Tick()
    {
        if (_poisonCurrentTime < Time.time)
        {
            _poisonCurrentTime = Time.time + _poisonTickTime;
            StartCoroutine(characterSound.BasicSound(3));
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
        StartCoroutine(characterSound.BasicSound(1));
        if (health <= 0 && isAlive)
        {
            if (animator && !animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            {
                animator.SetBool("Dead", true);
                animator.SetTrigger("DeathTrigger");
            }
            isAlive = false;
        }
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
    }

    public IEnumerator Attack()
    {
        ////Justin - TODO:Find a better method.

        List<string> ListOfEnemies = ServiceLocator.Get<ObjectPoolManager>().GetKeys();
        //float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        GameObject target = gameObject.GetComponent<PlayerController>().GetLockedOnTarget();

        closestEnemy = target;

        if (closestEnemy && Vector3.Distance(transform.position, closestEnemy.transform.position) <= attackRange)
        {
            if(closestEnemy.CompareTag("Enemy"))
            {
                closestEnemy?.GetComponent<Enemy>()?.TakeDamage(attack, true, DamageType.Normal);
                gameObject.GetComponent<PlayerController>().SwitchAutoLock(closestEnemy);
            }
            else if(closestEnemy.CompareTag("Boss"))
            {
                closestEnemy?.GetComponent<Boss>()?.TakeDamage(attack, true, DamageType.Normal);
                gameObject.GetComponent<PlayerController>().SwitchAutoLock(closestEnemy);
            }

            //audioSource.PlayOneShot(attackEffect, 0.75f);
            audioManager.PlaySfx(attackEffect);
            basicHitParticle.Play();
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
        poisonIndicator.GetComponent<Transform>().localScale = new Vector3(poisonRange * 2.0f, 0.007460861f, poisonRange * 2.0f);
        poisonIndicator.SetActive(true);
        List<string> ListOfEnemies = ServiceLocator.Get<ObjectPoolManager>().GetKeys();

        foreach (var enemy in ListOfEnemies)
        {
            List<GameObject> gameObjects = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects(enemy);
            foreach (var go in gameObjects)
            {
                float distance = Vector3.Distance(transform.position, go.transform.position);
                if (distance <= poisonRange)
                {
                    if(go.CompareTag("Enemy"))
                    {
                        go.GetComponent<Enemy>()?.TakeDamage(initialPoisonAttackDamage, true, DamageType.Poison);
                        go.GetComponent<Enemy>()?.SetPoison(poisonDamage, poisonTickTime, poisonTotalTime);
                    }
                    else if(go.CompareTag("Boss"))
                    {
                        go.GetComponent<Boss>()?.TakeDamage(initialPoisonAttackDamage, true, DamageType.Poison);
                        go.GetComponent<Boss>()?.SetPoison(poisonDamage, poisonTickTime, poisonTotalTime);
                    }

                }
            }
        }
        audioManager.PlaySfx(poisonEffect);
        poisonAttack.SetActive(true);
        GetComponent<PlayerController>().isUsingAbility = true;
        yield return new WaitForSeconds(1.0f);
        poisonIndicator.SetActive(!poisonIndicator.activeSelf);
        poisonAttack.SetActive(false);
        GetComponent<PlayerController>().isUsingAbility = false;
        yield return null;
    }

    public IEnumerator UltimateAttack()
    {
        if (_ultimateCharge != 100.0f)
            yield break;
        ////Justin - TODO:Find a better method.
        ultimateIndicator.GetComponent<Transform>().localScale = new Vector3(ultimateRange, 0.007460861f, ultimateRange);
        ultimateIndicator.SetActive(true);
        animator.SetTrigger("Ultimate");
        GetComponent<PlayerController>().isUsingAbility = true;
        GetComponent<PlayerController>().isUsingUltimate = true;
        StartCoroutine(ServiceLocator.Get<GameManager>().ShakeCamera(_duration,_shakeAmount,_decrFactor));
        yield return new WaitForSeconds(ultimateDelay);
        _uiManager.enableFadeOut(true);
        ultimateIndicator.SetActive(false);
        _ultimateCharge = 0.0f;
        List<string> ListOfEnemies = ServiceLocator.Get<ObjectPoolManager>().GetKeys();
        foreach (var enemy in ListOfEnemies)
        {
            List<GameObject> gameObjects = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects(enemy);
            foreach (var go in gameObjects)
            {
                float distance = Vector3.Distance(transform.position, go.transform.position);
                if (distance <= ultimateRange * 0.5f)
                {
                    if (go.CompareTag("Enemy"))
                    {
                        go.GetComponent<Enemy>()?.TakeDamage(ultimateDamage, true, DamageType.Ultimate);
                        go.GetComponent<Enemy>()?.SetPoison(ultimateTickDamage, ultimateTickTime, ultimateTotalTime);
                    }
                    else if(go.CompareTag("Boss"))
                    {
                        go.GetComponent<Boss>()?.TakeDamage(ultimateDamage, true, DamageType.Ultimate);
                        go.GetComponent<Boss>()?.SetPoison(ultimateTickDamage, ultimateTickTime, ultimateTotalTime);
                    }

                }
            }
        }
        //audioSource.PlayOneShot(UltimateEffect, 0.95f);
        audioManager.PlaySfx(UltimateEffect);
        yield return new WaitUntil(() =>  animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        GetComponent<PlayerController>().isUsingAbility = false;
        GetComponent<PlayerController>().isUsingUltimate = false;
    }

    public IEnumerator IntimidateAttack(/*GameObject enemy*/)
    {
        //if (enemy && Vector3.Distance(transform.position, enemy.transform.position) < attackRange)
        //{
        //    Instantiate(Lighting, enemy.transform.position, Quaternion.identity);
        //    Instantiate(LightingOnGround, gameObject.transform.position, Quaternion.identity);
        //    //audioSource.PlayOneShot(LightingEffectSound, 0.5f);
        //    audioManager.PlaySfx(LightingEffectSound);
        //    enemy.GetComponent<Enemy>()._Order = Order.Stunned;
        //    enemy.GetComponent<Enemy>().stunTime = Time.time + intimdateStunTime;
        //}
        GetComponent<PlayerController>().isUsingAbility = true;
        stunIndicator.GetComponent<Transform>().localScale = new Vector3(stunRange * 2.0f, 0.007460861f, stunRange * 2.0f);
        stunIndicator.SetActive(true);
        Instantiate(LightingOnGround, gameObject.transform.position, Quaternion.identity);
        audioManager.PlaySfx(LightingEffectSound, 0.5f);

        List<string> ListOfEnemies = ServiceLocator.Get<ObjectPoolManager>().GetKeys();

        foreach (var enemy in ListOfEnemies)
        {
            List<GameObject> gameObjects = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects(enemy);
            foreach (var go in gameObjects)
            {
                if(go.CompareTag("Enemy"))
                {
                    if (!go.GetComponent<Enemy>())
                        continue;
                    float distance = Vector3.Distance(transform.position, go.transform.position);
                    if (distance < stunRange)
                    {
                        Instantiate(Lighting, go.transform.position, Quaternion.identity);
                        go.GetComponent<Enemy>()._Order = Order.Stunned;
                        go.GetComponent<Enemy>().stunParticle.Play();
                        go.GetComponent<Enemy>().stunTime = Time.time + intimdateStunTime;
                    }
                }
                else if(go.CompareTag("Boss"))
                {
                    if (!go.GetComponent<Boss>())
                        continue;
                    float distance = Vector3.Distance(transform.position, go.transform.position);
                    if (distance < stunRange)
                    {
                        Instantiate(Lighting, go.transform.position, Quaternion.identity);
                        go.GetComponent<Boss>()._Order = Boss_Order.Stunned;
                        go.GetComponent<Boss>().stunParticle.Play();
                        go.GetComponent<Boss>().stunTime = Time.time + intimdateStunTime;
                    }
                }

            }
        }
        yield return new WaitForSeconds(1.0f);
        stunIndicator.SetActive(false);
        GetComponent<PlayerController>().isUsingAbility = false;
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
        if (!_isStoring)
        {
            _isStoring = true;
            GameObject heal = Instantiate(healingEffect, gameObject.transform.position, Quaternion.identity) as GameObject;
            StartCoroutine(characterSound.BasicSound(6));

            yield return new WaitForSeconds(1.0f);
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
        //if (animator && !animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        //    animator.SetBool("Dead", true);
        //isAlive = false;
        //yield return new WaitForSeconds(3.0f);
        yield return null;
    }

    #endregion

    public void FullUltimate()
    {
        _ultimateCharge = 100.0f;
    }
}
