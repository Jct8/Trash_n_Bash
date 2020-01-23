using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkunkAbility : MonoBehaviour, IEnemyAbilities
{
    [SerializeField] private float _skunksPoisonTickTime = 3.0f;
    [SerializeField] private float _skunksPoisonDamage = 1.0f;
    [SerializeField] private float _skunksPoisonRange = 5.0f;
    [SerializeField] private float _skunksPoisonTotaltime = 3.0f;

    public void GroupAttack()
    {
        return;
    }

    public void PoisonAOE(GameObject player)
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= _skunksPoisonRange)
        {
            player.GetComponent<Player>().SetPoisoned(_skunksPoisonDamage, _skunksPoisonTickTime, _skunksPoisonTotaltime);
        }
    }

}
