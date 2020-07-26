using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial1 : MonoBehaviour
{
    public GameObject exclaimationImage;
    public GameObject backgroundImage;
    public GameObject enemyMaskImage;
    public GameObject enemyButton;
    public GameObject fleasButton;
    public GameObject stunButton;
    public GameObject ultButton;
    public GameObject handImage;
    public GameObject handPointAtFleasImage;
    public GameObject handPointAtStunImage;
    public GameObject handPointAtUltImage;

    // Cache references
    Player player;
    PlayerController playerController;
    ObjectPoolManager objectPoolManager;
    Image waveTimerBar;

    List<float> waveTimes;

    float distanceToFirstEnemy = 12.0f;
    float currentDelay = 0.0f;
    int currentSequence = 0;
    bool isDelayed = false;

    // Start is called before the first frame update
    void Start()
    {
        player = ServiceLocator.Get<LevelManager>().playerInstance.GetComponent<Player>();
        playerController = ServiceLocator.Get<LevelManager>().playerInstance.GetComponent<PlayerController>();
        objectPoolManager = ServiceLocator.Get<ObjectPoolManager>();
        waveTimerBar = ServiceLocator.Get<UIManager>().waveTimerBar;
        waveTimes = ServiceLocator.Get<UIManager>().waveTimes;
        exclaimationImage.SetActive(false);
        enemyButton.SetActive(false);
        playerController.EnablePoisonAttack(false);
        playerController.EnableUltAttack(false);
        playerController.EnableIntimidateAttack(false);
        playerController.enableControls = false;
        currentSequence = 0;
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
            case 13:
                Sequence13();
                break;
            case 14:
                Sequence14();
                break;
            case 15:
                Sequence15();
                break;
            default:
                break;
        }
    }

    public void IncrementSequence()
    {
        currentSequence++;
    }

    void Sequence0() // Wait for first enemy
    {
        List<string> ListOfEnemies = objectPoolManager.GetKeys();
        foreach (var enemy in ListOfEnemies)
        {
            List<GameObject> gameObjects = objectPoolManager.GetActiveObjects(enemy);
            foreach (var go in gameObjects)
            {
                float distance = Vector2.Distance(player.gameObject.transform.position, go.transform.position);
                if (distance <= distanceToFirstEnemy)
                {
                    currentSequence++;
                    exclaimationImage.SetActive(true);
                    Time.timeScale = 0.0f;
                    currentDelay = Time.realtimeSinceStartup + 1.5f;
                    isDelayed = true;
                }
            }
        }
    }

    void Sequence1() // dealay first tutorial
    {
        backgroundImage.SetActive(true);
        currentDelay = Time.realtimeSinceStartup + 1.5f;
        isDelayed = true;
        currentSequence++;
    }

    void Sequence2() // show first tutorial to click
    {
        backgroundImage.SetActive(true);
        exclaimationImage.SetActive(false);
        enemyButton.SetActive(true);
    }

    void Sequence3() // after click first tutorial
    {
        enemyButton.SetActive(false);
        enemyMaskImage.SetActive(true);
        backgroundImage.SetActive(false);
        handImage.SetActive(true);
        playerController.enableControls = true;
        if (playerController.IsLockedOn)
        {
            currentSequence++;
            handImage.SetActive(false);
            enemyMaskImage.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }

    void Sequence4() // check for first wave
    {
        if (waveTimerBar.fillAmount > waveTimes[1])
        {
            enemyMaskImage.SetActive(false);
            backgroundImage.SetActive(true);
            //fleasButton.SetActive(true);
            Time.timeScale = 0.0f;
            currentDelay = Time.realtimeSinceStartup + 1.5f;
            isDelayed = true;
            currentSequence++;
        }
    }

    void Sequence5() // show button
    {
        fleasButton.SetActive(true);
    }

    void Sequence6() // Show hand point to fleas
    {
        Time.timeScale = 1.0f;
        fleasButton.SetActive(false);
        backgroundImage.SetActive(false);
        handPointAtFleasImage.SetActive(true);
        playerController.EnablePoisonAttack(true);
        currentSequence++;
    }

    void Sequence7() // wait for flea button clicked
    {
        if (playerController.usedFleasAbility)
        {
            handPointAtFleasImage.SetActive(false);
            currentSequence++;
        }
    }

    void Sequence8() // check for second wave
    {
        if (waveTimerBar.fillAmount > waveTimes[2])
        {
            backgroundImage.SetActive(true);
            Time.timeScale = 0.0f;
            playerController.enableControls = false;
            currentDelay = Time.realtimeSinceStartup + 1.5f;
            isDelayed = true;
            currentSequence++;
        }
    }

    void Sequence9() // delay for second tutorial - show button
    {
        stunButton.SetActive(true);
    }

    void Sequence10() // Show hand point to stun
    {
        playerController.enableControls = true;
        Time.timeScale = 1.0f;
        stunButton.SetActive(false);
        backgroundImage.SetActive(false);
        handPointAtStunImage.SetActive(true);
        playerController.EnableIntimidateAttack(true);
        currentSequence++;
    }

    void Sequence11() // wait for stun button clicked
    {
        if (playerController.usedStunAbility)
        {
            handPointAtStunImage.SetActive(false);
            currentSequence++;
        }
    }

    void Sequence12() // check for third wave
    {
        if (waveTimerBar.fillAmount > 0.8f)
        {
            playerController.enableControls = false;
            backgroundImage.SetActive(true);
            ultButton.SetActive(true);
            Time.timeScale = 0.0f;

            currentDelay = Time.realtimeSinceStartup + 1.5f;
            isDelayed = true;
            currentSequence++;
        }
    }

    void Sequence13() // show ult button
    {
        ultButton.SetActive(true);
    }

    void Sequence14() // Show hand point to ult
    {
        playerController.enableControls = true;
        Time.timeScale = 1.0f;
        ultButton.SetActive(false);
        backgroundImage.SetActive(false);
        handPointAtUltImage.SetActive(true);
        playerController.EnableUltAttack(true);
        player._ultimateCharge = 100;
        currentSequence++;
    }

    void Sequence15() // wait for ult button clicked
    {
        if (playerController.isUsingUltimate)
        {
            handPointAtUltImage.SetActive(false);
            currentSequence++;
        }
    }
}
