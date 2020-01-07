using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    public List<GameObject> starList;

    private void Awake()
    {
        for(int i = 0; i< ServiceLocator.Get<LevelManager>().GetStarRating();i++)
        {
            starList[i].SetActive(true);
        }
        GameObject.Find("EnemyText").GetComponent<Text>().text = "Enemies Defeated: " + ServiceLocator.Get<LevelManager>().enemyDeathCount;
    }
}
