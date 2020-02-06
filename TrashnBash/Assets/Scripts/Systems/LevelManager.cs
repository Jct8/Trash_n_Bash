using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject playerInstance;
    public GameObject towerInstance;

    public int levelNumber;
    public int enemyDeathCount;

    public float playerHealth = 100.0f;
    public float towerHealth = 100.0f;

    public bool isTutorial = false;

    public LevelManager Initialize()
    {
        return this;
    }

    public void ClearLevel()
    {
        SaveData();
        ResetLevel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
    }

    public void Restart()
    {
        UIManager uiManager = ServiceLocator.Get<UIManager>();
        PauseGame();
        ClearLevel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        uiManager.enableFadeOut();
        uiManager.Reset();
    }

    public void ReturnToMainMenu()
    {
        UIManager uiManager = ServiceLocator.Get<UIManager>();
        uiManager.attackImg.SetActive(true);
        uiManager.poisonImg.SetActive(true);
        uiManager.intimidateImg.SetActive(true);
        uiManager.ultImg.SetActive(true);
        PauseGame();
        ClearLevel();
        SceneManager.LoadScene("MainMenu");
    }

    public void PauseGame()
    {
        GameObject pauseScreen = ServiceLocator.Get<UIManager>().pauseScreen;
        pauseScreen.SetActive(!pauseScreen.activeSelf);

        if (pauseScreen.activeSelf)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }

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
        if (playerInstance.GetComponent<Player>().Health <= 0.0f || towerInstance.GetComponent<Tower>().fullHealth <= 0.0f)
        {
            return true;
        }
        return false;
    }

    public bool CheckWinCondition()
    {
        if (enemyDeathCount >= 50)
        {
            if (playerInstance != null)
                playerHealth = playerInstance.GetComponent<Player>().Health;
            if (towerInstance != null)
                towerHealth = towerInstance.GetComponent<Tower>().fullHealth;
            return true;
        }
        return false;
    }

    public int GetStarRating()
    {
        float average = (playerHealth + towerHealth) * 0.5f;

        if (average < 60.0f)
            return 1;
        else if (average >= 60.0f && average < 80.0f)
            return 2;
        else
            return 3;
    }

    public void ResetLevel()
    {
        UIManager uiManager = ServiceLocator.Get<UIManager>();
        uiManager.enableFadeOut();
        if (playerInstance == null || towerInstance == null)
        {
            playerInstance = GameObject.FindGameObjectWithTag("Player");
            towerInstance = GameObject.FindGameObjectWithTag("Tower");
        }
        enemyDeathCount = 0;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<Enemy>().rigid.velocity = Vector3.zero;
            enemies[i].GetComponent<Enemy>().killed?.Invoke();
        }
        uiManager.Reset();
        isTutorial = false;
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
