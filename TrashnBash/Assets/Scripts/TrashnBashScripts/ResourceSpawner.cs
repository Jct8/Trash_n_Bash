using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceSpawner : MonoBehaviour
{
    [Header("Trash can")]
    public GameObject resourcePrefab;
    public GameObject coolTimeGO;
    public Image coolTimeImage;
    public List<GameObject> trashModels;
    public float totalCoolTime = 15;
    private float currentCoolTime = 0;
    private bool completedCoolTime = false;

    void Start()
    {
        currentCoolTime = 0;
        completedCoolTime = false;
        coolTimeImage.fillAmount = 0;
    }

    public GameObject GetResource()
    {
        if(completedCoolTime)
        {
            GameObject resource = Instantiate(resourcePrefab, gameObject.transform.position, Quaternion.identity) as GameObject;

            //random trash model
            UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
            int randomNumber = UnityEngine.Random.Range(0, trashModels.Count - 1);
            GameObject resourceModel = Instantiate(trashModels[randomNumber], gameObject.transform.position, Quaternion.identity) as GameObject;
            resourceModel.transform.parent = resource.transform;

            currentCoolTime = 0;
            completedCoolTime = false;
            coolTimeImage.fillAmount = 0;
            return resource;
        }
        else
        {
            return null;
        }

    }

    void Update()
    {
        coolTimeGO.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
        if (coolTimeImage.fillAmount >= 1)
        {
            completedCoolTime = true;
            currentCoolTime += totalCoolTime + Time.time;

        }
        else
        {
            coolTimeImage.fillAmount += 1 / totalCoolTime * Time.deltaTime;
        }
    }
}