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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        UIManager uiManager = ServiceLocator.Get<UIManager>();
        PauseGame();
        //ResetLevel();
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
        //ResetLevel();
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
        if (enemyDeathCount >= 40)
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
        if(SceneManager.GetActiveScene().buildIndex == 3)
        {
            ServiceLocator.Get<GameManager>().changeGameState(GameManager.GameState.Tutorial);
        }
        else
        {
            ServiceLocator.Get<GameManager>().changeGameState(GameManager.GameState.GamePlay);
        }


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
        playerInstance.GetComponent<Player>().health = playerInstance.GetComponent<Player>()._maxHealth;
        //Upgrade options
        GameManager gameManager = ServiceLocator.Get<GameManager>();
        UpgradeStats upgradeStats = ServiceLocator.Get<UpgradeStats>();
        //Long Ranged Upgrade
        int rangedLevel = gameManager.upgradeLevelsDictionary[UpgradeMenu.Upgrade.Ranged] - 1;
        if (rangedLevel >= 0)
            towerInstance.GetComponent<Tower>().range += upgradeStats.towerRange[rangedLevel];
        //Barricade Upgrade
        BarricadeSpawner barricadeSpawner = GameObject.FindGameObjectWithTag("BarricadeSpawner")?.GetComponent<BarricadeSpawner>();
        int barricadeLevel = gameManager.upgradeLevelsDictionary[UpgradeMenu.Upgrade.Barricades] - 1;
        if (barricadeLevel >= 0 && barricadeSpawner)
            barricadeSpawner.baseBarricadeCost -= upgradeStats.barricadeCostReduction[barricadeLevel];
        //Wife tower Upgrade
        Tower wife = GameObject.FindGameObjectWithTag("Wife").GetComponent<Tower>();
        int wifeLevel = gameManager.upgradeLevelsDictionary[UpgradeMenu.Upgrade.ExtraProjectiles] - 1;
        if (wifeLevel >= 0)
        {
            wife.isShooting = true;
            wife.attackRate -= upgradeStats.throwingSpeed[wifeLevel];
        }
        //Specific Target Upgrade
        int specficLevel = gameManager.upgradeLevelsDictionary[UpgradeMenu.Upgrade.TargetEnemy] - 1;
        if (specficLevel >= 0)
            towerInstance.GetComponent<Tower>().specificEnemy = upgradeStats.targetEnemy[specficLevel];
        // Fire Upgrade
        int fireLevel = gameManager.upgradeLevelsDictionary[UpgradeMenu.Upgrade.FireProjectile] - 1;
        if (fireLevel >= 0)
        {
            towerInstance.GetComponent<Tower>().damageType = DamageType.Poison;
            towerInstance.GetComponent<Tower>().fireDuration = upgradeStats.fireDuration[fireLevel];
        }

        //UpdateTowerTrashCount
        towerInstance.GetComponent<Tower>().fullHealth = gameManager._houseHP;
    }

    public void LoadData()
    {
        return;
    }
}
