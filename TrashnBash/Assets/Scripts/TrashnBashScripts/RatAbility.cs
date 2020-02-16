using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAbility : MonoBehaviour, IEnemyAbilities
{
    public int limit = 3;
    public int numbers = 0;

    public void Flying(Transform wayPoint, Order order)
    {
        return;
    }

    public void GroupAttack()
    {
        string name = gameObject.GetComponent<Enemy>().Name;
        foreach (GameObject rat in ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects(name))
        {
            if (numbers <= limit)
            {
                if (rat.GetComponent<Enemy>()._Order != Order.Barricade)
                    rat.GetComponent<Enemy>()._Order = Order.Fight;
            }
            else
            {
                numbers = 0;
                return;
            }
            numbers++;
        }
    }

    public void PlayDead(GameObject player)
    {
        return;
    }

    public void PoisonAOE(GameObject player)
    {
        return;
    }
}
