using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAbility : MonoBehaviour, IEnemyAbilities
{
    public void GroupAttack()
    {
        int limit = 3;
        int numbers = 0;
        string name = gameObject.GetComponent<Enemy>().Name;
        foreach (GameObject rat in ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects(name))
        {
            if (numbers < limit)
            {
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
