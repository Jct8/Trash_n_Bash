using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public List<EnemySpawner> Spawners;
    public WayPointManager WayPointManager;
    public TutorialManager tutorialManager;

    private void Awake()
    {
        foreach (var spawner in Spawners)
        {
            spawner.Init(WayPointManager.GetPath(spawner._pathID));
        }
    }

    void Start()
    {
        foreach (var spawner in Spawners)
        {
            if (spawner.StartOnSceneLoad == true)
            {
                spawner.StartSpawner();
            }
        }
    }

    public void StartSpawer(EnemySpawner spawner)
    {
        tutorialManager.AddCount(spawner._numberOfWave * spawner._enemiesPerWave);
        spawner.StartSpawner();
    }

    public void ResetSpawners()
    {
        foreach (var spawner in Spawners)
        {
            spawner.ResetSpawner();
        }
    }
    public void StartAllSpawners()
    {
        ResetSpawners();
        foreach (var spawner in Spawners)
        {
            spawner.StartSpawner();
        }
    }

}
