using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAbility : MonoBehaviour, IEnemyAbilities
{
    [SerializeField][Tooltip("Limit amount of Rats for using a skill of rats")] private int limit = 3;
    private int numbers = 0;

    public void Flying(Transform wayPoint)
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
