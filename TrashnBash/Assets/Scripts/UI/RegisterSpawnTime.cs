using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterSpawnTime : MonoBehaviour
{
    public int StartSpawnTime = 0;
    public int MaximumSpawnTime = 0;
    public List<GameObject> spawners = new List<GameObject>();
    private void Start()
    {
        foreach(Transform child in transform)
        {
            spawners.Add(child.gameObject);
        }

        StartSpawnTime = spawners[0].GetComponent<EnemySpawner>()._secondStartDelay;
        foreach(var child in spawners)
        {
            int time = child.GetComponent<EnemySpawner>()._secondStartDelay;
            if (StartSpawnTime > time)
                StartSpawnTime = time;
            if (MaximumSpawnTime < time)
                MaximumSpawnTime = time;
        }

        spawners.Clear();
    }
}
