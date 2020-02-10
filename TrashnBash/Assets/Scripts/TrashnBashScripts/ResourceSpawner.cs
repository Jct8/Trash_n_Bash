using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    public GameObject resourcePrefab;

    public GameObject GetResource()
    {
        GameObject resource = Instantiate(resourcePrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
        return resource;
    }

}