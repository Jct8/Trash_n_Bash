using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject playerInstance;
    public GameObject towerInstance;

    public int levelNumber;
    public int enemyDeathCount;

    public float playerHealth = 100.0f;
    public float towerHealth = 100.0f;

    public LevelManager Initialize()
    {
        return this;
    }

    public void ClearLevel()
    {
        SaveData();
        ResetLevel(); 
    }

    public void IncreaseEnemyDeathCount(int increment)
    {
        enemyDeathCount += increment;
        //Debug.Log("Enemy Death Count:" + enemyDeathCount);
    }

    public bool CheckLoseCondition()
    {
        if (playerInstance == null || towerInstance == null)
        {
            return false;
        }
        if (playerInstance.GetComponent<Player>().Health <= 0.0f || towerInstance.GetComponent<Tower>()._FullHealth <= 0.0f)
        {
            return true;
        }
        return false;
    }

    public bool CheckWinCondition()
    {
        if (enemyDeathCount == 10)
        {
            if(playerInstance != null)
                playerHealth = playerInstance.GetComponent<Player>().Health;
            if(towerInstance != null)
                towerHealth = towerInstance.GetComponent<Tower>()._FullHealth;
            return true;
        }
        return false;
    }

    public int GetStarRating()
    {
        float average = (playerHealth + towerHealth) *0.5f;

        if (average < 60.0f)
            return 1;
        else if (average >= 60.0f && average < 80.0f)
            return 2;
        else
            return 3;
    }

    public void ResetLevel()
    {
        if (playerInstance == null || towerInstance == null)
        {
            playerInstance = GameObject.FindGameObjectWithTag("Player");
            towerInstance = GameObject.FindGameObjectWithTag("Tower");
        }
        enemyDeathCount = 0;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            enemy.GetComponent<Enemy>().rigid.velocity = Vector3.zero;
            enemy.GetComponent<Enemy>().killed?.Invoke();
        }
        playerHealth = 100.0f;
        towerHealth = 100.0f;
    }

    public void SaveData()
    {
        return;
    }

    public void LoadData()
    {
        return;
    }
}
