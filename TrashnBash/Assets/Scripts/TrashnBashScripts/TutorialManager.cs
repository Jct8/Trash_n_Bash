using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public List<GameObject> UISequences;

    private int currentSequence = 0;
    private int numEnemiesToKill = 0;
    private bool isSpawnStarted = false;

    private LevelManager levelManager;

    private void Start()
    {
        levelManager = ServiceLocator.Get<LevelManager>();
        UISequences[currentSequence].SetActive(true);
    }

    private void Update()
    {
        if (numEnemiesToKill == levelManager.enemyDeathCount && isSpawnStarted)
        {
            currentSequence++;
            if (UISequences.Count - 1 >= currentSequence)
            {
                UISequences[currentSequence].SetActive(true);
            }
            isSpawnStarted = false;
        }
    }

    public void SetEnemySpawner(List<EnemySpawner> enemySpawner)
    {
        isSpawnStarted = true;
        foreach (var item in enemySpawner)
        {
            numEnemiesToKill += item._numberOfWave * item._enemiesPerWave;
        }
    }

    public void AddCount(int total)
    {
        numEnemiesToKill += total;
    }

}
