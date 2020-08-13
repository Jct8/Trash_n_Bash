using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public List<GameObject> UISequences;
    public Button barricadeCreateBtn;

    private int currentSequence = 0;
    private int numEnemiesToKill = 0;
    private bool isSpawnStarted = false;

    private LevelManager levelManager;
    private EnemySpawnManager enemySpawnManager;
    private Barricade barricade = null;

    private bool isplaced = false;

    private void Start()
    {
        enemySpawnManager = FindObjectOfType<EnemySpawnManager>();

        levelManager = ServiceLocator.Get<LevelManager>();
        levelManager.isTutorial = true;
        PlayerController player = levelManager.playerInstance.GetComponent<PlayerController>();
        player.attackEnabled = true;
        player.poisonAttackEnabled = false;
        player.intimidateAttackEnabled = false;
        player.ultimateAttackEnabled = false;
        UIManager uiManager = ServiceLocator.Get<UIManager>();
        //uiManager.attackImg.SetActive(true);
        uiManager.poisonImg.SetActive(false);
        uiManager.intimidateImg.SetActive(false);
        uiManager.ultImg.SetActive(false);
        uiManager.waveTimerBar.gameObject.SetActive(false);
        uiManager.timerObject.gameObject.SetActive(false);
        StartCoroutine(StartSequence(1.0f));
    }

    private void Update()
    {
        if (numEnemiesToKill <= levelManager.enemyDeathCount && isSpawnStarted)
        {
            IncrementSequence();
            isSpawnStarted = false;

            enemySpawnManager.ResetSpawners();
        }
        if (barricade)
        {
            if (barricade.isPlaced == true && isplaced == false)
            {
                //IncrementSequence();
                //barricade.TakeFullDamage();
                isplaced = true;
                EndTutorial();
            }
        }
    }

    IEnumerator StartSequence(float delay)
    {
        yield return new WaitForSeconds(delay);
        UISequences[currentSequence].SetActive(true);
    }

    public void IncrementSequence()
    {
        currentSequence++;
        if (UISequences.Count - 1 >= currentSequence)
        {
            UISequences[currentSequence].SetActive(true);
        }

    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SetEnemySpawner(EnemySpawner enemySpawner)
    {
        isSpawnStarted = true;
        numEnemiesToKill += enemySpawner._numberOfWave * enemySpawner._enemiesPerWave;
        //foreach (var item in enemySpawner)
        //{
        //    numEnemiesToKill += item._numberOfWave * item._enemiesPerWave;
        //}
    }

    public void AddCount(int total)
    {
        isSpawnStarted = true;
        numEnemiesToKill += total;
    }

    public void AddBarricade(Barricade inputBarricade)
    {
        currentSequence++;
        if (UISequences.Count - 1 >= currentSequence)
        {
            UISequences[currentSequence].SetActive(true);
        }
        barricade = inputBarricade;
    }

    public void EndTutorial()
    {
        currentSequence++;
        if (UISequences.Count - 1 >= currentSequence)
        {
            UISequences[currentSequence].SetActive(true);
        }
        GameObject barricadeManager = GameObject.FindGameObjectWithTag("BarricadeSpawner");
        ServiceLocator.Get<LevelManager>().isTutorial = false;
        LevelManager levelManager =  ServiceLocator.Get<LevelManager>();

        levelManager.playerInstance.GetComponent<PlayerController>().EnableIntimidateAttack();
        levelManager.playerInstance.GetComponent<PlayerController>().EnableUltAttack();
        levelManager.playerInstance.GetComponent<PlayerController>().EnablePoisonAttack();

        UIManager uiManager = ServiceLocator.Get<UIManager>();

        uiManager.waveTimerBar.gameObject.SetActive(true);
        uiManager.timerObject.gameObject.SetActive(true);

        uiManager.StartTimer();

        enemySpawnManager.StartAllSpawners();
    }

}
