using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarricadeSpawner : MonoBehaviour
{
    public GameObject barricadePrefab;
    public int barricadeLimit = 5;

    private int totalBarricades = 0;

    public GameObject GetBarricade()
    {
        if (totalBarricades <= barricadeLimit)
        {
            GameObject barricade = Instantiate(barricadePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            totalBarricades++;
            return barricade;
        }
        return null;
    }
}
