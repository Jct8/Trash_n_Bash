using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public List<EnemySpawner> Spawners { get; set; }
    private TutorialManager tutorialManager;

    private void Awake()
    {
        Spawners = new List<EnemySpawner>(FindObjectsOfType<EnemySpawner>());
        tutorialManager = FindObjectOfType<TutorialManager>();
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
        if (tutorialManager != null)
            tutorialManager.AddCount(spawner._numberOfWave * spawner._enemiesPerWave);
        
        spawner.StartSpawner();
    }

    public void ResetSpawners()
    {
        ServiceLocator.Get<UIManager>().waveTimerBar.fillAmount = 1;
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
            ServiceLocator.Get<UIManager>().totalWave += spawner._enemiesPerWave;
        }
        ServiceLocator.Get<UIManager>().currentWave = ServiceLocator.Get<UIManager>().totalWave;
        foreach (var spawner in Spawners)
        {
            spawner.StartSpawner();
            
        }
    }

}
