using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkunkAbility : MonoBehaviour, IEnemyAbilities
{
    [SerializeField][Tooltip("Tick time for poison skill")] private float _skunksPoisonTickTime = 3.0f;
    [SerializeField][Tooltip("Value to damage poison for Skunk")] private float _skunksPoisonDamage = 1.0f;
    [SerializeField][Tooltip("Area of poison for Skunk")] private float _skunksPoisonRange = 5.0f;
    [SerializeField][Tooltip("Total time for poison skill")] private float _skunksPoisonTotaltime = 3.0f;

    public GameObject poisonArea;
    public ParticleSystem poisonParticle;
    public AudioClip fartEffect;
    private float skunkCooldown = 3.0f;
    private float currentSkunkCooldown = 3.0f;
    private void Start()
    {
        poisonArea.SetActive(false);
        VariableLoader variableLoader = ServiceLocator.Get<VariableLoader>();
        if (variableLoader.useGoogleSheets)
        {
            _skunksPoisonDamage = variableLoader.EnemyStats["Skunks"]["Damage"];
            _skunksPoisonRange = variableLoader.EnemyStats["Skunks"]["Range"];
            skunkCooldown = variableLoader.EnemyStats["Skunks"]["CoolDown"];
        }
        
    }

    private void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player)
        {
            if (Vector3.Distance(poisonArea.transform.position, player.transform.position)  >= _skunksPoisonRange)
            {
                poisonArea.SetActive(false);
                if (poisonParticle.isPlaying)
                    poisonParticle.Stop();
            }
        }

    }

    public void GroupAttack()
    {
        return;
    }

    public void PoisonAOE()
    {
        GameObject player = ServiceLocator.Get<LevelManager>().playerInstance;
        if (!player)
            return;

        AudioManager audioManager = ServiceLocator.Get<AudioManager>();

        poisonArea.GetComponent<Transform>().localScale = new Vector3(_skunksPoisonRange, 0.00746f, _skunksPoisonRange);
        if (Vector3.Distance(poisonArea.transform.position, player.transform.position) < _skunksPoisonRange )
        {
            //poisonArea.SetActive(true);
            if(!poisonParticle.isPlaying)
            {
                var pSmain = poisonParticle.main;
                poisonParticle.Simulate(1.0f);
                poisonParticle.Play();
            }
            if(currentSkunkCooldown < Time.time)
            {
                audioManager.PlaySfx(fartEffect);
                currentSkunkCooldown = Time.time + skunkCooldown;
                /// Updated to divide total damage by skunk tick time to 
                player.GetComponent<Player>().SetPoisoned(_skunksPoisonDamage / _skunksPoisonTickTime, 1.0f, _skunksPoisonTotaltime);
            }
        }
    }

    public void Flying(Transform wayPoint)
    {
        return;
    }

    public void PlayDead()
    {
        return;
    }
}
