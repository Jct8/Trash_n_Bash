using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [Header("Trash can")]
    public GameObject resourcePrefab;
    public List<GameObject> trashModels;
    [SerializeField] private int _amountofLimit = 5;

    private int currentResource = 0;

    public GameObject GetResource()
    {
        if(currentResource < _amountofLimit)
        {
            currentResource++;
            GameObject resource = Instantiate(resourcePrefab, gameObject.transform.position, Quaternion.identity) as GameObject;

            //random trash model
            UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
            int randomNumber = UnityEngine.Random.Range(0, trashModels.Count - 1);
            GameObject resourceModel = Instantiate(trashModels[randomNumber], gameObject.transform.position, Quaternion.identity) as GameObject;
            resourceModel.transform.parent = resource.transform;
            return resource;
        }
        else
        {
            return null;
        }
    }
}