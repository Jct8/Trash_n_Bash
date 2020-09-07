using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Boss : MonoBehaviour, ICharacterAction
{
    #region Variables
    [Header("UI Health")]
    public Image healthBar;
    public Image CooltimeBar;
    public GameObject healthBarGO;
    public GameObject CoolTimeGO;
    public GameObject popUp;
    public GameObject poison;
    public GameObject hitEffect;
    public GameObject Lighting;

    public ParticleSystem fireEffect;
    public ParticleSystem stunParticle;

    public GameObject poisonAttack;
    public AudioClip poisonEffect;
    private GameObject player;
    public GameObject _targetIndicator;
    public GameObject _stunIndicator;
    public GameObject _poisonIndicator;
    public GameObject LightingOnGround;
    private GameObject _ObjectofBarricade;
    private DataLoader _DataLoader;
    private JsonDataSource _EnemyData;
    private EnemySpawner.EnemyPath _Path;
    public NavMeshAgent _Agent;
    public AudioClip LightingEffectSound;

    public Boss_Order _Order { get; set; }
    //public StateMachine stateMachine;

    public float health;
    public float stunTime = 0.0f;
    private bool _IsDead = false;
    private bool _IsAttacked = false;
    private bool _isPoisoned = false;
    private bool _isOnFire = false;
    private bool _startSummon = false;
    public string _DataSource;

    public AudioClip attackEffect;
    private AudioSource audioSource;
    private AudioManager audioManager;

    public string Name { get { return _Name; } private set { } }
    public bool IsDead { get { return _IsDead; } set { _IsDead = value; } }
    [Header("Enemy Status")]
    public float MaxHealth = 18.0f;
    [SerializeField] private string _Name;
    [SerializeField] private float _Attack = 1.0f;
    [SerializeField] private float _Money;
    [SerializeField] private float _Speed = 1.0f;
    [SerializeField] private float _AttackCoolTime = 3.0f;
    [SerializeField] private float _enemyAttackRange = 4.0f;

    [SerializeField] private float _totalPoisonAttackCooltime = 10.0f;
    [SerializeField] private float _totalStunAttackCooltime = 5.0f;

    [SerializeField] private float _PoisonSkillDamage = 2.0f;
    [SerializeField] private float _PoisonSkillRange = 5.0f;
    [SerializeField] private float _PoisonSkillTickTime = 1.0f;
    [SerializeField] private float _TotalPoisonSkillTime = 3.0f;

    [SerializeField] private float _stunSkillRange = 3.0f;
    [SerializeField] private float _stunSkillTime = 3.0f;

    private int _HowManySpawn = 0;
    private int _Next = 0;
    private List<GameObject> Allspawners = new List<GameObject>();

    private float _currentPoisonAttackCooltime = 0.0f;
    private float _currentStunAttackCooltime = 0.0f;

    private bool _waitingBehaviour = false;
    private bool _isRunning = false;
    private int current = 0;
    private float timer = 0.0f;

    private float _EndDistance = 3.0f;
    private float _MaximumAngle = 45.0f;
    private float _MaximumDistance = 5.0f;
    private float _distanceToBarricade;
    private float _poisonDamage = 0.0f;
    private float _poisonTotalTime = 0.0f;
    private float _poisonTickTime = 0.0f;
    private float _poisonCurrentDuration = 0.0f;

    private float _FireDamage = 0.0f;
    private float _FireTotalTime = 0.0f;
    private float _FireTickTime = 0.0f;
    private float _FireCurrentDuration = 0.0f;

    [Header("Etc")]
    public Rigidbody rigid;
    public Action killed;
    private Action OnRecycle;
    public int score = 0;
    IEnemyAbilities enemyAbilities;
    ICharacterSound characterSound;

    private Animator animator;

    #endregion

    #region UnityFunctions
    private void Start()
    {
        //stateMachine = new StateMachine();
        //stateMachine.ChangeState(new MoveState(this));
        audioSource = GetComponent<AudioSource>();
        audioManager = ServiceLocator.Get<AudioManager>();
        CooltimeBar.fillAmount = 0;
        enemyAbilities = GetComponent<IEnemyAbilities>();
        characterSound = GetComponent<ICharacterSound>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        player = ServiceLocator.Get<LevelManager>().playerInstance;
    }

    void Update()
    {
        healthBarGO.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
        CoolTimeGO.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);

        if (player == null || _IsDead)
        {
            _Agent.isStopped = true;
            _targetIndicator.SetActive(false);
            return;
        }
        if (!_IsDead)
        {
            if (_isPoisoned)
                CheckPoison();
            if (_isOnFire)
                CheckFire();

            if (_Order == Boss_Order.Stunned)
            {
                if (healthBar.fillAmount <= 0.666f && !_waitingBehaviour)
                {
                    _Order = Boss_Order.Back;
                    return;
                }
                CooltimeBar.fillAmount = 0;
                _Agent.isStopped = true;
                if (animator)
                    animator.speed = 0.0f;
                if (stunTime < Time.time)
                {
                    stunParticle.Stop();
                    _Agent.isStopped = false;
                    _Order = Boss_Order.Fight;
                    if (animator)
                        animator.speed = 1.0f;
                }
                return;
            }
            else if(_Order == Boss_Order.Move)
            {
                LookAt(player.transform.position, player);
                if (animator)
                {
                    if(!_isRunning)
                    {
                        animator.SetTrigger("Run");
                        _isRunning = true;
                    }
                }

                if(isInRangeOfWayPoint(player.transform, _enemyAttackRange))
                {
                    _isRunning = false;
                    _Order = Boss_Order.Fight;
                }
                else
                {
                    CooltimeBar.fillAmount = 0;
                    _IsAttacked = false;
                }
            }
            else if (_Order == Boss_Order.Fight)
            {
                gameObject.GetComponent<CapsuleCollider>().enabled = true;
                if (healthBar.fillAmount <= 0.666f && !_waitingBehaviour)
                {
                    _Order = Boss_Order.Back;
                    _isRunning = false;
                    return;
                }
                else if(healthBar.fillAmount <= 0.333f)
                {
                    if(!_startSummon)
                    {
                        StartCoroutine(beginSummon());
                        _startSummon = true;
                    }
                }

                int random = UnityEngine.Random.Range(1, 100);
                if (random > 50 && _currentPoisonAttackCooltime == _totalPoisonAttackCooltime)
                {
                    Debug.Log("Posion");
                    _Order = Boss_Order.PoisonAttack;
                    CooltimeBar.fillAmount = 0;
                }
                else if (random <= 50 && _currentStunAttackCooltime == _totalStunAttackCooltime)
                {
                    Debug.Log("Stun");
                    _Order = Boss_Order.StunAttack;
                    CooltimeBar.fillAmount = 0;
                }
                else
                {
                    _Order = Boss_Order.Fight;
                }

                if (ChargingCoolDown())
                {
                    _isRunning = false;
                    StartCoroutine("Attack");
                }
                else
                {
                    if (animator)
                    {
                        if (!_isRunning)
                        {
                            animator.SetTrigger("Idle");
                            _isRunning = true;
                        }
                    }
                }
            }
            else if (_Order == Boss_Order.PoisonAttack)
            {
                if(ChargingCoolDown())
                {
                    _isRunning = false;
                    animator.SetTrigger("Fleas");
                    StartCoroutine(PoisonAttack());
                    _poisonIndicator.SetActive(false);
                    _Order = Boss_Order.Fight;
                    _currentPoisonAttackCooltime = 0;
                }
                else
                {
                    _poisonIndicator.GetComponent<Transform>().localScale = new Vector3(_PoisonSkillRange, 0.007460861f, _PoisonSkillRange);
                    _poisonIndicator.SetActive(true);
                    if (!_isRunning)
                    {
                        animator.SetTrigger("Idle");
                        _isRunning = true;
                    }
                }
            }
            else if(_Order == Boss_Order.StunAttack)
            {
                if (ChargingCoolDown())
                {
                    _isRunning = false;
                    animator.SetTrigger("Intimidate");
                    StartCoroutine(IntimidateAttack());
                    _stunIndicator.SetActive(false);
                    _Order = Boss_Order.Fight;
                    _currentStunAttackCooltime = 0;
                    _currentPoisonAttackCooltime = 0;
                }
                else
                {
                    _stunIndicator.GetComponent<Transform>().localScale = new Vector3(_stunSkillRange, 0.007460861f, _stunSkillRange);
                    _stunIndicator.SetActive(true);
                    if (!_isRunning)
                    {
                        animator.SetTrigger("Idle");
                        _isRunning = true;
                    }
                }
            }
            else if (_Order == Boss_Order.Back)
            {
                _Agent.isStopped = false;

                if(!_isRunning)
                {
                    animator.SetTrigger("Run");
                    _isRunning = true;
                }

                CooltimeBar.fillAmount = 0;
                Transform _Desination = _Path.WayPoints[0];
                _Agent.SetDestination(_Desination.position);
                if (isInRangeOfWayPoint(_Desination, _EndDistance))
                {
                    LookAt(player.transform.position, player);
                    _isRunning = false;
                    _Order = Boss_Order.Waiting;
                }
                else
                    LookAt(_Desination.gameObject.transform.position, _Desination.gameObject);
            }
            else if(_Order == Boss_Order.Waiting)
            {
                gameObject.GetComponent<CapsuleCollider>().enabled = false;
                _waitingBehaviour = true;
                if(!_startSummon)
                {
                    StartCoroutine(beginSummon());
                    _startSummon = true;
                }
            }

            timer += Time.deltaTime;

            if(timer > 1.0f)
            {

                if (_totalPoisonAttackCooltime > _currentPoisonAttackCooltime)
                    _currentPoisonAttackCooltime += 1.0f;

                if (_totalStunAttackCooltime > _currentStunAttackCooltime)
                    _currentStunAttackCooltime += 1.0f;

                timer = 0.0f;
            }

        }
    }
    #endregion

    #region Initiliaztion

    public void Initialize(EnemySpawner.EnemyPath path, Action Recycle)
    {
        poison.SetActive(_isPoisoned);
        _Path = path;
        killed += Recycle;
        _DataLoader = ServiceLocator.Get<DataLoader>();
        _EnemyData = _DataLoader.GetDataSourceById(_DataSource) as JsonDataSource;

        _Name = System.Convert.ToString(_EnemyData.DataDictionary["Name"]);

        _Order = Boss_Order.Move;

        _Agent = GetComponent<NavMeshAgent>();
        _Agent.speed = _Speed;
        _IsDead = false;

        //Reset Variable
        health = MaxHealth;
        healthBar.fillAmount = health / MaxHealth;
        CooltimeBar.fillAmount = 0;
        rigid = gameObject.GetComponent<Rigidbody>();
        _targetIndicator = transform.Find("TargetIndicator").gameObject;

        gameObject.GetComponent<Boss>().SwitchOnTargetIndicator(false);
        _IsAttacked = false;
        _isPoisoned = false;
        _startSummon = false;
        VariableLoader variableLoader = ServiceLocator.Get<VariableLoader>();
        if (variableLoader.useGoogleSheets)
        {
            MaxHealth = variableLoader.EnemyStats[_Name]["HP"];
            _Speed = variableLoader.EnemyStats[_Name]["Speed"];
            _Attack = variableLoader.EnemyStats[_Name]["Steal"];
            _Attack = variableLoader.EnemyStats[_Name]["Damage"];
            _AttackCoolTime = variableLoader.EnemyStats[_Name]["CoolDown"];
            _enemyAttackRange = variableLoader.EnemyStats[_Name]["Range"];

            health = MaxHealth;
            healthBar.fillAmount = health / MaxHealth;
        }
    }

    public void ResetStatus()
    {
        if (animator)
            animator.SetBool("Dead", false);
        healthBarGO.SetActive(true);
        _Agent.isStopped = false;
        _IsDead = false;
        _Order = Boss_Order.Move;
        health = MaxHealth;
        healthBar.fillAmount = health / MaxHealth;
        CooltimeBar.fillAmount = 0;
        rigid.velocity = Vector3.zero;
        _isPoisoned = false;
        _IsAttacked = false;
        poison.SetActive(_isPoisoned);
        player = ServiceLocator.Get<LevelManager>().playerInstance;
    }

    #endregion

    #region Check and Ultility Functions

    private bool ChargingCoolDown()
    {
        StartCoroutine(AttackCoolDown());
        if (CooltimeBar.fillAmount >= 1)
        {
            return true;
        }
        return false;
    }

    private IEnumerator AttackCoolDown()
    {
        CooltimeBar.fillAmount += 1 / _AttackCoolTime * Time.deltaTime;
        yield return new WaitForSeconds(_AttackCoolTime);
    }

    public void SwitchOnTargetIndicator(bool turnOn)
    {
        _targetIndicator.SetActive(turnOn);
    }

    public void SwitchEnemyDead(bool active)
    {
        _IsDead = active;
    }

    private bool isInRangeOfWayPoint(Transform destination, float range)
    {
        // Range is enemy's area, it could be enemy's attack range, or enemy's width
        // if enemy's area reachs to inside of distance, they stop moving
        float distance = Vector3.Distance(transform.position, destination.position);
        return (distance <= range) ? _Agent.isStopped = true : _Agent.isStopped = false;
    }

    private void LookAt(Vector3 obj, GameObject go)
    {
        _Agent.SetDestination(go.transform.position);
        obj.y = transform.position.y;
        transform.LookAt(obj);
    }

    #endregion

    #region TakeDamage
    public void TakeDamage(float Dmg, bool isHero, DamageType type)
    {
        if (_Order == Boss_Order.Back || _Order == Boss_Order.Waiting)
            return;
        if (_IsDead)
            return;
        health -= Dmg;
        StartCoroutine(characterSound.BasicSound(1));
        popUp.GetComponent<TextMesh>().text = Dmg.ToString();

        switch (type)
        {
            case DamageType.Normal:
                popUp.GetComponent<TextMesh>().color = new Color(1.0f, 0.0f, 0.0f);
                GameObject hit = Instantiate(hitEffect, transform.position, Quaternion.identity) as GameObject;
                break;
            case DamageType.Poison:
                popUp.GetComponent<TextMesh>().color = new Color(0.0f, 1.0f, 0.0f);
                break;
            case DamageType.Ultimate:
                popUp.GetComponent<TextMesh>().color = new Color(0.0f, 0.0f, 1.0f);
                break;
            case DamageType.Fire:
                popUp.GetComponent<TextMesh>().color = new Color(1.0f, 0.0f, 0.0f);
                break;
        }
        Instantiate(popUp, transform.position, Camera.main.transform.rotation, transform);

        healthBar.fillAmount = health / MaxHealth;


        if (health <= 0.0f)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(DeathAnimation());
            }
            if (!_IsDead)
                player.GetComponent<Player>().IncrementUltCharge();

            _IsDead = true;
            _isPoisoned = false;
            _IsAttacked = false;
            stunParticle.Stop();
        }
    }

    private void CheckPoison()
    {
        if (_poisonCurrentDuration < Time.time)
        {
            _poisonCurrentDuration = Time.time + _poisonTickTime;

            StartCoroutine(characterSound.BasicSound(3));
            TakeDamage(_poisonDamage, true, DamageType.Poison);
        }
        if (_poisonTotalTime < Time.time)
        {
            _isPoisoned = false;
            poison.SetActive(_isPoisoned);
            return;
        }
    }

    public void SetPoison(float damage, float tickTime, float totalTime)
    {
        if (_isPoisoned)
        {
            return;
        }
        _poisonDamage = damage;
        _poisonTickTime = tickTime;
        _poisonCurrentDuration = Time.time;
        _poisonTotalTime = Time.time + totalTime;
        _isPoisoned = true;
        StartCoroutine(characterSound.BasicSound(2));
        poison.SetActive(_isPoisoned);
    }

    private void CheckFire()
    {
        if (_FireCurrentDuration < Time.time)
        {
            _FireCurrentDuration = Time.time + _FireTickTime;
            TakeDamage(_FireDamage, false, DamageType.Fire);
        }
        if (_FireTotalTime < Time.time)
        {
            _isOnFire = false;
            fireEffect.Stop();
            return;
        }
    }

    public void SetFire(float damage, float tickTime, float totalTime)
    {
        if (_isOnFire)
            return;
        _FireDamage = damage;
        _FireTickTime = tickTime;
        _FireCurrentDuration = Time.time;
        _FireTotalTime = Time.time + totalTime;
        _isOnFire = true;

        fireEffect.Play();
    }

    #endregion
    
    #region Attacks

    public IEnumerator Attack()
    {
        if (!_IsAttacked)
        {
            if (animator)
                animator.SetTrigger("Scratch");
            _IsAttacked = true;
        }
        player = ServiceLocator.Get<LevelManager>().playerInstance;
        if (FrontAttack(player.transform) && !_IsDead)
        {
            player.GetComponent<Player>().TakeDamage(_Attack, false, DamageType.Enemy);
        }
        StartCoroutine(characterSound.BasicSound(0));
        CooltimeBar.fillAmount = 0;
        _IsAttacked = false;
        _Order = Boss_Order.Move;
        yield return null;
    }

    public IEnumerator PoisonAttack()
    {
        _IsAttacked = true;
        player = ServiceLocator.Get<LevelManager>().playerInstance;
        if(Vector3.Distance(this.transform.position, player.transform.position) < _PoisonSkillRange)
        {
            player.GetComponent<Player>().SetPoisoned(_PoisonSkillDamage, _PoisonSkillTickTime, _TotalPoisonSkillTime);
        }
        audioManager.PlaySfx(poisonEffect);
        poisonAttack.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        poisonAttack.SetActive(false);
        CooltimeBar.fillAmount = 0;
        _IsAttacked = false;
        yield return null;
    }

    public IEnumerator IntimidateAttack()
    {
        Instantiate(LightingOnGround, gameObject.transform.position, Quaternion.identity);
        audioManager.PlaySfx(LightingEffectSound, 0.5f);

        player = ServiceLocator.Get<LevelManager>().playerInstance;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance < _stunSkillRange)
        {
            Instantiate(Lighting, player.transform.position, Quaternion.identity);
            player.GetComponent<Player>().stunParticle.Play();
            player.GetComponent<PlayerController>().isStunned = true;
            player.GetComponent<PlayerController>().stunTime = Time.time + _stunSkillTime;
        }
        
        yield return new WaitForSeconds(1.0f);
        CooltimeBar.fillAmount = 0;
    }

    bool FrontAttack(Transform target)
    {
        Vector3 Coneforward = transform.TransformDirection(Vector3.forward);
        Vector3 ConeToTarget = target.position - transform.position;
        ConeToTarget.Normalize();
        float _Distance = Vector3.Distance(transform.position, target.transform.position);

        if ((Vector3.Angle(ConeToTarget, Coneforward) < _MaximumAngle) && (_Distance < _MaximumDistance))
        {
            return true;
        }

        return false;
    }

    private IEnumerator beginSummon()
    {
        Allspawners.Clear();
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
        if (spawners == null)
        {
            Debug.Log("Couldn't find");
            yield return null;
        }
        foreach (GameObject obj in spawners)
        {
            Allspawners.Add(obj);
        }
        GameObject BossSpawner = GameObject.Find("BigRaccoon");
        Allspawners.Remove(BossSpawner);

        if (_Next > 0)
        {
            foreach (GameObject obj in Allspawners)
            {
                obj.GetComponent<EnemySpawner>().ResetSpawner();
                obj.GetComponent<EnemySpawner>().StartSpawner();
            }
        }
        else
        {
            foreach (GameObject obj in Allspawners)
            {
                obj.GetComponent<EnemySpawner>().ResetSpawner();
                obj.GetComponent<EnemySpawner>().StartSpawner();
                _HowManySpawn += obj.GetComponent<EnemySpawner>()._numberOfWave;
            }

            while (current < _HowManySpawn)
            {
                current = 0;
                foreach (var s in Allspawners)
                {
                    current += s.GetComponent<EnemySpawner>()._currentWave;
                }
                if (current == _HowManySpawn) break;
                yield return new WaitForSeconds(1.0f);
            }
            Activate();
        }
        yield return null;
    }

    public void Activate()
    {
        current = 0;
        _Agent.isStopped = true;
        _Order = Boss_Order.Move;
        _startSummon = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = true;
        _Next++;
    }

    public void Recycle(GameObject obj)
    {
        ServiceLocator.Get<ObjectPoolManager>().RecycleObject(obj);
    }
    #endregion

    #region Animations

    public IEnumerator DeathAnimation()
    {
        if (animator)
        {
            animator.speed = 1.0f;
            animator.SetTrigger("DeathTrigger");
        }
        _Agent.isStopped = true;
        healthBarGO.SetActive(false);
        ServiceLocator.Get<LevelManager>().IncreaseEnemyDeathCount(1);
        ServiceLocator.Get<LevelManager>().IncreaseScore(score);
        yield return new WaitForSeconds(3.0f);
        killed?.Invoke();
        yield return null;
    }

    public void UpdateAnimation()
    {
        throw new NotImplementedException();
    }
    #endregion
}
