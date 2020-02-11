using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    public GameObject resourcePrefab;
    public List<GameObject> trashModels;

    public GameObject GetResource()
    {
        GameObject resource = Instantiate(resourcePrefab, gameObject.transform.position, Quaternion.identity) as GameObject;

        //random trash model
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
        int randomNumber = UnityEngine.Random.Range(0, trashModels.Count - 1);
        GameObject resourceModel = Instantiate(trashModels[randomNumber], gameObject.transform.position, Quaternion.identity) as GameObject;
        resourceModel.transform.parent = resource.transform;

        return resource;
    }

}