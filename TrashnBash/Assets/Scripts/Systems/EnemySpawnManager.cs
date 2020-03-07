using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    private List<EnemySpawner> Spawners;

    private void Awake()
    {
        Spawners = new List<EnemySpawner>(GetComponentsInChildren<EnemySpawner>());
    }

    void Start()
    {
        foreach(var spawner in Spawners)
        {
            if(spawner.StartOnSceneLoad == true)
            {
                spawner.StartSpawner();
            }
        }
    }

    public void StartTutorialSpawner(EnemySpawner spawner)
    {
        FindObjectOfType<TutorialManager>().AddCount(spawner._numberOfWave * spawner._enemiesPerWave);
        spawner.StartSpawner();
    }

    public void ResetSpawners()
    {
        foreach (var spawner in Spawners)
        {
            spawner.ResetSpawner();
        }
    }
}
