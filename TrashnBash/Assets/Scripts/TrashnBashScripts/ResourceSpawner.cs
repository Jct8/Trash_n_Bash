using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    public GameObject resourcePrefab;

    public int numberOfResource = 0;
    public int limitofResource = 2;

    private void Start()
    {
        createResource();
    }

    public void createResource()
    {
        for (int i = 0; i <= limitofResource; i++)
        {
            if (numberOfResource <= limitofResource)
            {
                float randomNumber = UnityEngine.Random.Range(-1.0f, 1.1f);
                GameObject resource = Instantiate(resourcePrefab, new Vector3
                    (gameObject.transform.position.x + randomNumber, 
                    gameObject.transform.position.y, 
                    gameObject.transform.position.z + randomNumber), 
                    Quaternion.identity) as GameObject;
            }
            else
            {
                break;
            }
        }

        gameObject.SetActive(false);
    }

}