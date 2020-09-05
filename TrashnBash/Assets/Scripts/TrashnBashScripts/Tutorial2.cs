using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial2 : MonoBehaviour
{
    public GameObject backgroundImage;
    public GameObject barricadeMaskImage;
    public GameObject healthMaskImage;
    public GameObject trashMaskImage;
    public GameObject playerHealthMaskImage;
    public GameObject playerHealthButtonMaskImage;

    public GameObject barricadeButton;
    public GameObject trashButton;
    public GameObject healthButton;

    public GameObject handPointAtBarricade;
    public GameObject handPointAtTrash;
    public GameObject handPointAtTowerHealth;

    public Transform handPosition1;
    public Transform handPosition2;

    // Cache references
    Player player;
    PlayerController playerController;
    Tower tower;
    ObjectPoolManager objectPoolManager;
    Image waveTimerBar;

    List<float> waveTimes;

    public float minHealth = 20.0f;
    float distanceToFirstEnemy = 12.0f;
    float currentDelay = 0.0f;
    int currentSequence = 0;
    bool isDelayed = false;

    public bool usedTrashToHeal = false;
    public bool barricadePlaced = false;
    public bool usedHeal = false;


    // Start is called before the first frame update
    void Start()
    {
        player = ServiceLocator.Get<LevelManager>().playerInstance.GetComponent<Player>();
        playerController = ServiceLocator.Get<LevelManager>().playerInstance.GetComponent<PlayerController>();
        tower = ServiceLocator.Get<LevelManager>().towerInstance.GetComponent<Tower>();
        objectPoolManager = ServiceLocator.Get<ObjectPoolManager>();
        waveTimerBar = ServiceLocator.Get<UIManager>().waveTimerBar;
        waveTimes = ServiceLocator.Get<UIManager>().waveTimes;
        ServiceLocator.Get<UIManager>().fade.SetActive(false);
        barricadeButton.SetActive(false);
        playerController.EnablePoisonAttack(false);
        playerController.EnableUltAttack(false);
        playerController.EnableIntimidateAttack(false);
        playerController.enableControls = false;

        Time.timeScale = 0.0f;
        currentDelay = Time.realtimeSinceStartup + 2.0f;
        isDelayed = true;

        ResourceSpawner[] resourceSpawner = FindObjectsOfType<ResourceSpawner>();
        foreach (var item in resourceSpawner)
            item.enableSpawnResources = false;

        tower.enablePlayerRegainHealth = false;

        //Reset
        backgroundImage.SetActive(false);
        barricadeMaskImage.SetActive(false);
        healthMaskImage.SetActive(false);
        trashMaskImage.SetActive(false);
        playerHealthMaskImage.SetActive(false);
        playerHealthButtonMaskImage.SetActive(false);

        barricadeButton.SetActive(false);
        trashButton.SetActive(false);
        healthButton.SetActive(false);

        handPointAtBarricade.SetActive(false);
        handPointAtTrash.SetActive(false);
        handPointAtTowerHealth.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDelayed)
        {
            if (currentDelay < Time.realtimeSinceStartup)
                isDelayed = false;
            else
                return;
        }

        switch (currentSequence)
        {
            case 0:
                Sequence0();
                break;
            case 1:
                Sequence1();
                break;
            case 2:
                Sequence2();
                break;
            case 3:
                Sequence3();
                break;
            case 4:
                Sequence4();
                break;
            case 5:
                Sequence5();
                break;
            case 6:
                Sequence6();
                break;
            case 7:
                Sequence7();
                break;
            case 8:
                Sequence8();
                break;
            case 9:
                Sequence9();
                break;
            case 10:
                Sequence10();
                break;
            case 11:
                Sequence11();
                break;
            case 12:
                Sequence12();
                break;
            default:
                break;
        }
    }

    public void IncrementSequence()
    {
        currentSequence++;
    }

    void Sequence0() // Wait in starting scene
    {
        backgroundImage.SetActive(true);
        currentDelay = Time.realtimeSinceStartup + 1.0f;
        isDelayed = true;
        currentSequence++;
    }

    void Sequence1() // show barricade help button
    {
        backgroundImage.SetActive(true);
        barricadeButton.SetActive(true);
    }

    void Sequence2() // show barricade mask
    {
        backgroundImage.SetActive(false);
        barricadeButton.SetActive(false);
        barricadeMaskImage.SetActive(true);
        currentDelay = Time.realtimeSinceStartup + 1.0f;
        isDelayed = true;
        playerController.enableControls = true;
        currentSequence++;
    }

    void Sequence3() // move hand pointer
    {
        handPointAtBarricade.SetActive(true);
        var dir = handPosition2.position - handPointAtBarricade.transform.position;
        handPointAtBarricade.transform.position += Vector3.Normalize(dir) * Time.unscaledDeltaTime * 500.0f;
        if (Vector3.Distance(handPosition2.position,handPointAtBarricade.transform.position) < 5.0f)
            handPointAtBarricade.transform.position = handPosition1.position;
        if (barricadePlaced)
        {
            handPointAtBarricade.SetActive(false);
            barricadeMaskImage.SetActive(false);
            Time.timeScale = 1.0f;
            currentSequence++;

            playerController.EnablePoisonAttack(true);
            playerController.EnableUltAttack(true);
            playerController.EnableIntimidateAttack(true);
        }
    }

    void Sequence4() // wait for low health
    {
        if(tower.fullHealth <= minHealth)
        {
            healthMaskImage.SetActive(true);
            Time.timeScale = 0.0f;

            currentDelay = Time.realtimeSinceStartup + 2.0f;
            isDelayed = true;

            ResourceSpawner[] resourceSpawner = FindObjectsOfType<ResourceSpawner>();
            foreach (var item in resourceSpawner)
            {
                item.enableSpawnResources = true;
                item.coolTimeImage.fillAmount = 1.0f;
            }

            currentSequence++;
        }
    }

    void Sequence5() // Show health button
    {
        //healthMaskImage.SetActive(false);
        trashButton.SetActive(true);
        //backgroundImage.SetActive(true);
    }

    void Sequence6() // Show hand and trash mask after button is pressed
    {
        trashMaskImage.SetActive(true);
        trashButton.SetActive(false);
        healthMaskImage.SetActive(false);
        handPointAtTrash.SetActive(true);

        currentSequence++;
    }

    void Sequence7() // Wait for user to click trash
    {
        if(usedTrashToHeal)
        {
            currentDelay = Time.realtimeSinceStartup + 2.0f;
            handPointAtTrash.SetActive(false);
            isDelayed = true;
            currentSequence++;
        }
    }

    void Sequence8() // after user clicked trash
    {
        Time.timeScale = 1.0f;
        trashMaskImage.SetActive(false);
        currentSequence++;
    }

    void Sequence9() // check for low tower health
    {
        if(player.health <= tower.minimumPlayerHealth)
        {
            tower.enablePlayerRegainHealth = true;
            playerHealthMaskImage.SetActive(true);
            currentDelay = Time.realtimeSinceStartup + 2.0f;
            isDelayed = true;
            currentSequence++;
            Time.timeScale = 0.0f;
        }
    }

    void Sequence10()  // Show button for health
    {
        healthButton.SetActive(true);
    }

    void Sequence11() // Show pointer to tower health
    {
        playerHealthMaskImage.SetActive(false);
        healthButton.SetActive(false);
        playerHealthButtonMaskImage.SetActive(true);
        handPointAtTowerHealth.SetActive(true);
        currentSequence++;
    }

    void Sequence12() // wait for player to click heal
    {
        if(usedHeal)
        {
            Time.timeScale = 1.0f;
            playerHealthButtonMaskImage.SetActive(false);
            handPointAtTowerHealth.SetActive(false);
            currentSequence++;
        }
    }
}
