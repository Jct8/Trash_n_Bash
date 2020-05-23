using SheetCodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BarricadeSpawner : MonoBehaviour
{
    public GameObject barricadePrefab;
    public GameObject signifier;
    public Transform barricadeSpawnPosition;
    public GameObject trashImgPrefab;

    public int barricadeLimit = 5;
    public float spawnCoolDownTime = 5.0f;
    [HideInInspector]
    public float spawnCoolDownBeforeUpgrade = 2.0f; //Used for upgrades
    [HideInInspector]
    public float spawnCoolDownAfterUpgrade = 2.0f;//Used for upgrades

    public float baseBarricadeCost = 10.0f;
    [HideInInspector]
    public float costBeforeUpgrade = 2.0f; //Used for upgrades
    [HideInInspector]
    public float costAfterUpgrade = 2.0f;//Used for upgrades

    private int totalBarricades = 0;
    private float currentTime = 0.0f;

    private bool isDragging = false;

    private void Start()
    {
        VariableLoader variableLoader = ServiceLocator.Get<VariableLoader>();
        if (variableLoader.useGoogleSheets)
        {
            baseBarricadeCost = variableLoader.BarriacdeStats["TrashCost"];
        }
        ServiceLocator.Get<GameManager>().barricadeSpawner = this;

        ///////////  Upgrades - Barricade Spawn Rate Improved  ///////////
        int level = ServiceLocator.Get<GameManager>().upgradeLevelsDictionary[UpgradeMenu.Upgrade.BarricadeSpawnRate];
        spawnCoolDownBeforeUpgrade = spawnCoolDownTime;
        UpgradesIdentifier upgradesIdentifier = ModelManager.UpgradesModel.GetUpgradeEnum(UpgradeMenu.Upgrade.BarricadeSpawnRate, level);
        if (level >= 1)
        {
            spawnCoolDownTime -= ModelManager.UpgradesModel.GetRecord(upgradesIdentifier).ModifierValue;
            spawnCoolDownAfterUpgrade = spawnCoolDownTime;
        }

        ///////////  Upgrades - Barricade Reduction Cost Upgrade ///////////
        int barricadeLevel = ServiceLocator.Get<GameManager>().upgradeLevelsDictionary[UpgradeMenu.Upgrade.BarricadeReductionCost];
        upgradesIdentifier = ModelManager.UpgradesModel.GetUpgradeEnum(UpgradeMenu.Upgrade.BarricadeReductionCost, barricadeLevel);
        costBeforeUpgrade = baseBarricadeCost;
        if (barricadeLevel >= 1)
        {
            baseBarricadeCost -= ModelManager.UpgradesModel.GetRecord(upgradesIdentifier).ModifierValue;
            costAfterUpgrade = baseBarricadeCost;
        }
        
    }

    private void Update()
    {
        if (currentTime < Time.time && totalBarricades < barricadeLimit)
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
                //barricade.transform.position = barricadeSpawnPosition.position;
                barricade.SetActive(false);

                GameObject trashImg = Instantiate(trashImgPrefab);
                trashImg.transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                trashImg.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
                trashImg.GetComponent<DragDrop>().isDragging = true;
                trashImg.GetComponent<DragDrop>().itemToBeDroped = barricade;

                currentTime = Time.time + spawnCoolDownTime;
            }
        }
    }

    public void ResetBarricade()
    {
        totalBarricades--;
        currentTime = Time.time;
        ServiceLocator.Get<LevelManager>().towerInstance.GetComponent<Tower>().HealTower(baseBarricadeCost);
    }

    public void RemoveUpgrade()
    {

    }

    public void ApplyUpgrade()
    {

    }
}
