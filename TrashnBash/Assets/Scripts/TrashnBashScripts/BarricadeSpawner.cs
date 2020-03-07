using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarricadeSpawner : MonoBehaviour
{
    public GameObject barricadePrefab;
    public int barricadeLimit = 5;
    public float baseBarricadeCost = 10.0f;

    private int totalBarricades = 0;

    public GameObject GetBarricade()
    {
        if(ServiceLocator.Get<LevelManager>().isTutorial)
        {
            if(totalBarricades < 1)
            {
                ServiceLocator.Get<LevelManager>().towerInstance.GetComponent<Tower>().TakeDamage(baseBarricadeCost);
                GameObject barricade = Instantiate(barricadePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                totalBarricades++;
                if (ServiceLocator.Get<LevelManager>().isTutorial)
                {
                    TutorialManager tutorialManager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<TutorialManager>();
                    tutorialManager.AddBarricade(barricade.GetComponent<Barricade>());
                }
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
}
