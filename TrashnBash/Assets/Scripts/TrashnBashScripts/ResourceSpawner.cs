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
    public float totalCoolTime = 15;
    public int limit = 3;
    private bool completedCoolTime = false;
    private int totalResourceTaken = 0;

    void Start()
    {
        completedCoolTime = false;
        totalResourceTaken = 0;
        coolTimeImage.fillAmount = 0;
    }

    public GameObject GetResource()
    {
        if(completedCoolTime)
        {
            if(totalCoolTime < limit)
            {
                GameObject resource = Instantiate(resourcePrefab, gameObject.transform.position, Quaternion.identity) as GameObject;

                //random trash model
                UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
                int randomNumber = UnityEngine.Random.Range(0, trashModels.Count - 1);
                GameObject resourceModel = Instantiate(trashModels[randomNumber], gameObject.transform.position, Quaternion.identity) as GameObject;
                resourceModel.transform.parent = resource.transform;
                totalResourceTaken++;
                completedCoolTime = false;
                return resource;
            }
            else
            {
                return null;
            }
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

        }
        else
        {
            coolTimeImage.fillAmount += 1 / totalCoolTime * Time.deltaTime;
        }
    }

    public void SpawnResource()
    {
        if(coolTimeImage.fillAmount >= 1)
        {
            GameObject resource = GetResource();
            if(resource)
            {
                resource.transform.position = ObjectPos.transform.position;
                coolTimeImage.fillAmount = 0;
            }
        }

    }
}