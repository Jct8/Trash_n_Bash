using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpossumAbility : MonoBehaviour, IEnemyAbilities
{
    [SerializeField] private float castingTime = 3.0f;
    private int _stack = 0;
    public void Flying(Transform wayPoint, Order order)
    {
        return;
    }

    public void GroupAttack()
    {
        return;
    }

    public void PlayDead(GameObject player)
    {
        if(_stack == 0)
        {
            _stack++;
            player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<PlayerController>().DeselectLockOn();
            gameObject.GetComponent<Enemy>().SwitchOnTargetIndicator(false);
            gameObject.GetComponent<Enemy>().SwitchEnemyDead(true);
            StartCoroutine("castingTimeforOpossum");
        }
        return;
    }

    public void PoisonAOE(GameObject player)
    {
        return;
    }

    private IEnumerator castingTimeforOpossum()
    {
        yield return new WaitForSeconds(castingTime);
        gameObject.GetComponent<Enemy>().SwitchEnemyDead(false);
        gameObject.GetComponent<Enemy>()._Agent.isStopped = false;
        yield return null;
    }

    private void OnEnable()
    {
        _stack = 0;
    }
}
