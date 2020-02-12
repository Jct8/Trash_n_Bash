using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAbility : MonoBehaviour, IEnemyAbilities
{
    public int limit = 3;
    public int numbers = 0;

    public void Flying(bool fly, Order order)
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

    public void PoisonAOE(GameObject player)
    {
        return;
    }
}
