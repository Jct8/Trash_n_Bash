using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpossumAbility : MonoBehaviour, IEnemyAbilities
{
    [SerializeField][Tooltip("Casting time to Play Dead")] private float castingTime = 4.0f;
    private int _stack = 0;
    public Animator animator;
    public void Flying(Transform wayPoint)
    {
        return;
    }

    public void GroupAttack()
    {
        return;
    }

    public void PlayDead()
    {
        GameObject player = ServiceLocator.Get<LevelManager>().playerInstance;

        if (!player)
            return;

        if (_stack == 0)
        {
            _stack++;

            gameObject.GetComponent<Enemy>().SwitchOnTargetIndicator(false);
            gameObject.GetComponent<Enemy>().SwitchEnemyDead(true);
            player.GetComponent<PlayerController>().CheckTargetLockedOn();
            player.GetComponent<PlayerController>().ActivateTargetLockedOn();
            StartCoroutine("castingTimeforOpossum");
        }
        return;
    }

    public void PoisonAOE()
    {
        return;
    }

    private IEnumerator castingTimeforOpossum()
    {
        animator.SetBool("isPlayDead", true);
        animator.SetBool("isWakeUp", false);
        yield return new WaitForSeconds(castingTime);
        animator.SetBool("isPlayDead", false);
        animator.SetBool("isWakeUp", true);
        gameObject.GetComponent<Enemy>().SwitchEnemyDead(false);
        gameObject.GetComponent<Enemy>()._Agent.isStopped = false;

        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isWakeUp", false);
        yield return null;
    }

    private void OnEnable()
    {
        _stack = 0;
    }
}
