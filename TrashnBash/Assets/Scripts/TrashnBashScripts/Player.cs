using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICharacterAction
{
    public float health = 100.0f;
    public float attack = 20.0f;
    public float attackRange = 20.0f;
    public float attackAngleRange = 45.0f;
    public float poisonDamage = 2.0f;
    public float poisonTotalTime = 2.0f;
    public float poisonTickTime = 3.0f;

    private float maxHealth = 100.0f;

    void Start()
    {
        maxHealth = health;
    }

    public void TakeDamage(float damage, bool isHero)
    {
        health -= damage;
    }

    public void Attack(float damage = 10.0f)
    {
        List<GameObject> gameObjectsRats = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects("Rats");
        List<GameObject> gameObjectsSkunks = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects("Skunks");

        //Justin - TODO:Find a better method.
        foreach (var go in gameObjectsRats)
        {
            Vector3 direction = (go.transform.position - transform.position );
            float distance = Vector2.Distance(transform.position, go.transform.position);
            float angle = Vector3.Angle(transform.forward, direction);
            if (Mathf.Abs(angle) < attackAngleRange && distance< attackRange)
            {
                go.GetComponent<Enemy>().TakeDamage(attack, true);
            }
        }
        foreach (var go in gameObjectsSkunks)
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

    public void PoisonAttack()
    {
        List<GameObject> gameObjectsRats = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects("Rats");
        List<GameObject> gameObjectsSkunks = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects("Skunks");

        //Justin - TODO:Find a better method.
        foreach (var go in gameObjectsRats)
        {
            Vector3 direction = (go.transform.position - transform.position);
            float distance = Vector2.Distance(transform.position, go.transform.position);
            float angle = Vector3.Angle(transform.forward, direction);
            if (Mathf.Abs(angle) < attackAngleRange && distance < attackRange)
            {
                go.GetComponent<Enemy>().SetPoison(poisonDamage,poisonTickTime,poisonTotalTime);
            }
        }
        foreach (var go in gameObjectsSkunks)
        {
            Vector3 direction = (go.transform.position - transform.position);
            float distance = Vector2.Distance(transform.position, go.transform.position);
            float angle = Vector3.Angle(transform.forward, direction);
            if (Mathf.Abs(angle) < attackAngleRange && distance < attackRange)
            {
                go.GetComponent<Enemy>().SetPoison(poisonDamage, poisonTickTime, poisonTotalTime);
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
