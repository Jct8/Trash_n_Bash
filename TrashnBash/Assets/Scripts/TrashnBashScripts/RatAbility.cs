using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAbility : MonoBehaviour, IEnemyAbilities
{
    public int limit = 3;
    public int numbers = 0;
    private bool _active = false;
    public void GroupAttack()
    {
        _active = ServiceLocator.Get<GameManager>()._enemySkillActived;
        string name = gameObject.GetComponent<Enemy>().Name;
        if(_active)
        {
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
                    _active = false;
                    return;
                }
                numbers++;
            }
        }
        else
        {
            return;
        }
    }

    public void PoisonAOE(GameObject player)
    {
        return;
    }
}
