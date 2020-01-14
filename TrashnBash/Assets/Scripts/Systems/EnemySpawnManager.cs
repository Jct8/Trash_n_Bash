using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public List<EnemySpawner> Spawners;
    public WayPointManager WayPointManager;

    private void Awake()
    {
        foreach(var spawner in Spawners)
        {
            spawner.Init(WayPointManager.GetPath(spawner._pathID));
        }
    }

    void Start()
    {
        foreach(var spawner in Spawners)
        {
            if(spawner.StartOnSceneLoad == true)
                spawner.StartSpawner();
        }
    }

    public void StartSpawer(EnemySpawner spawner)
    {
        spawner.StartSpawner();
    }
}
