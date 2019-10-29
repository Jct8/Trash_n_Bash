using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObjectPool : MonoBehaviour
{
    public string poolToTest = "Cube";
    public int numObjectsToSpawn = 10;

    void Start()
    {
        StartCoroutine("SpawnObjectRoutine");
    }

    private IEnumerator SpawnObjectRoutine()
    {
        int objectCounter = 0;
        while (true)
        {
            GameObject cube = ServiceLocator.Get<ObjectPoolManager>().GetObjectFromPool(poolToTest);
            cube.transform.position = transform.position;
            cube.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            objectCounter++;
        }
    }
}
