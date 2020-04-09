using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarricadeSpawner : MonoBehaviour
{
    public GameObject barricadePrefab;
    public GameObject signifier;
    public Transform barricadeSpawnPosition;

    public int barricadeLimit = 5;
    public float baseBarricadeCost = 10.0f;
    public float spawnCoolDownTime = 5.0f;

    private int totalBarricades = 0;
    private float currentTime = 0.0f;

    private void Update()
    {
        if (currentTime < Time.time)
            signifier.SetActive(true);
        else
            signifier.SetActive(false);
    }

    public GameObject GetBarricade()
    {
        if (ServiceLocator.Get<LevelManager>().isTutorial)
        {
            if (totalBarricades < 1)
            {
                ServiceLocator.Get<LevelManager>().towerInstance.GetComponent<Tower>().TakeDamage(baseBarricadeCost);
                GameObject barricade = Instantiate(barricadePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                totalBarricades++;
                TutorialManager tutorialManager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<TutorialManager>();
                tutorialManager.AddBarricade(barricade.GetComponent<Barricade>());
                return barricade;
            }
        }
        else
        {
            if (totalBarricades <= barricadeLimit)
            {
                ServiceLocator.Get<LevelManager>().towerInstance.GetComponent<Tower>().TakeDamage(baseBarricadeCost);
                GameObject barricade = Instantiate(barricadePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                totalBarricades++;
                return barricade;
            }
        }

        return null;
    }

    public void SpawnBarricade()
    {
        if (currentTime < Time.time)
        {
            GameObject barricade = GetBarricade();
            if (barricade)
            {
                barricade.transform.position = barricadeSpawnPosition.position;
                currentTime = Time.time + spawnCoolDownTime;
            }
        }
    }

}
