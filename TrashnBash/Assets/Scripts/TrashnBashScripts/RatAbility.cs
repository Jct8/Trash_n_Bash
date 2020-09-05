using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAbility : MonoBehaviour, IEnemyAbilities
{
    [SerializeField][Tooltip("Limit amount of Rats for using a skill of rats")] private int limit = 3;
    private int count = 0;
    public void Flying(Transform wayPoint)
    {
        return;
    }

    public void GroupAttack()
    {
        string name = gameObject.GetComponent<Enemy>().Name;

        List<GameObject> rats = ServiceLocator.Get<ObjectPoolManager>().GetActiveObjects(name);
        List<GameObject> nearest = new List<GameObject>();

        foreach (GameObject rat in rats)
        {
            if (rat.GetComponent<Enemy>().IsDead) continue;

            if (this.gameObject == rat.gameObject)
                continue;
            
            if (nearest.Count <= limit)
            {
                nearest.Add(rat);
            }
            for (int i = 0; i < nearest.Count; i++)
            {
                if (nearest[i].GetComponent<Enemy>()._Order != Order.Barricade)
                {
                    if (count == limit) break;
                    nearest[i].GetComponent<Enemy>()._Order = Order.Fight;
                    count++;
                }
            }

        }
        nearest.Clear();
        count = 0;
    }

    public void PlayDead()
    {
        return;
    }

    public void PoisonAOE()
    {
        return;
    }
}
