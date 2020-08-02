using SheetCodes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceSpawner : MonoBehaviour
{
    [Header("Trash can")]
    public GameObject ObjectPos;
    public GameObject resourcePrefab;
    public GameObject coolTimeGO;
    public Image coolTimeImage;
    public List<GameObject> trashModels;

    [SerializeField]
    private float healValue = 10.0f;
    public float totalCoolTime = 15.0f;
    [HideInInspector]
    public float coolTimeBeforeUpgrade = 15.0f;
    [HideInInspector]
    public float coolTimeAfterUpgrade = 15.0f;
    public int limit = 3;
    private bool completedCoolTime = false;
    private HapticFeedback hapticFeedback;

    // For tutorial
    Tutorial2 tutorial2;
    public bool enableSpawnResources = true;

    void Start()
    {
        completedCoolTime = false;
        coolTimeImage.fillAmount = 0;

        VariableLoader variableLoader = ServiceLocator.Get<VariableLoader>();
        if (variableLoader.useGoogleSheets)
        {
            //limit = (int) variableLoader.TrashCanStats["TrashLimit"];
            totalCoolTime = variableLoader.TrashCanStats["Cooldown"];
            healValue = variableLoader.TrashCanStats["AmountCollected"];
        }

        ///////////  Upgrades - Trash Spawn Rate Improved  ///////////
        int level = ServiceLocator.Get<GameManager>().upgradeLevelsDictionary[UpgradeMenu.Upgrade.TrashSpawnRate];
        UpgradesIdentifier upgradesIdentifier = ModelManager.UpgradesModel.GetUpgradeEnum(UpgradeMenu.Upgrade.TrashSpawnRate, level);
        coolTimeBeforeUpgrade = totalCoolTime;
        if (level >= 1)
        {
            totalCoolTime -= ModelManager.UpgradesModel.GetRecord(upgradesIdentifier).ModifierValue;
            coolTimeAfterUpgrade = totalCoolTime;
        }
        tutorial2 = GameObject.FindObjectOfType<Tutorial2>()?.GetComponent<Tutorial2>();
    }

    public GameObject GetResource()
    {
        //if(completedCoolTime)
        //{
        //    if(totalResourceTaken < limit)
        //    {
        //        GameObject resource = Instantiate(resourcePrefab, gameObject.transform.position, Quaternion.identity) as GameObject;

        //        //random trash model
        //        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
        //        int randomNumber = UnityEngine.Random.Range(0, trashModels.Count - 1);
        //        GameObject resourceModel = Instantiate(trashModels[randomNumber], gameObject.transform.position, Quaternion.identity) as GameObject;
        //        resourceModel.transform.parent = resource.transform;
        //        totalResourceTaken++;
        //        completedCoolTime = false;
        //        return resource;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        //else
        //{
        //    return null;
        //}
        return null;
    }

    void Update()
    {
        if(!enableSpawnResources)
        {
            coolTimeImage.fillAmount = 0.0f;
        }
        coolTimeGO.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
        if (coolTimeImage.fillAmount >= 1)
        {
            completedCoolTime = true;

        }
        else
        {
            coolTimeImage.fillAmount += 1 / totalCoolTime * Time.deltaTime;
        }
    }

    public void GettingResource()
    {
        if(coolTimeImage.fillAmount >= 1)
        {
            Tower tower = ServiceLocator.Get<LevelManager>().towerInstance.GetComponent<Tower>();
            tower.GetComponent<Tower>().HealTower(healValue);
            coolTimeImage.fillAmount = 0;
            hapticFeedback = GetComponent<HapticFeedback>();
            hapticFeedback?.Activate();
            // For tutorial
            if (tutorial2)
                tutorial2.usedTrashToHeal = true;
        }

    }
}