using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Canvas canvas;

    public GameObject player;
    public GameObject tower;
    public Image playerHealthBar;
    public Image waveTimerBar;
    public Text towerHealthPercentage;
    //public Animator AnimationTexture;

    //public GameObject PresentTextrue;
    public GameObject[] spawners;
    public GameObject pauseScreen;
    public GameObject optionsScreen;
    public Texture BasicTexture;
    public Texture SickTexture;
    public Texture PowerFulTexture;

    public Image loadingImg;
    public Image poisonCover;
    public Image intimidateCover;
    public Image ultCover;
    public Image repairIcon;
    public GameObject fade;
    public GameObject fadeDumster;
    public GameObject fadeTutorial;

    public GameObject poisonImg;
    public GameObject intimidateImg;
    public GameObject ultImg;
    public GameObject repairButton;
    public GameObject placeButton;
    public GameObject timerObject;

    public EndScreen endPanel;

    public Button pauseButton;
    public Button continueButton;
    public Button restartButton;
    public Button mainmenuButton;

    private float _TowerHP;
    private bool IsPower = false;
    public float maximumTimer;
    public float currentTimer = 0.0f;

    public List<GameObject> signifiersForWaves = new List<GameObject>();

    private void Start()
    {
        fade.SetActive(false);
        fadeDumster.SetActive(false);
        fadeTutorial.SetActive(false);
    }

    public void enableFadeOut()
    {
        fade.SetActive(true);
        fade.GetComponent<Animator>().Play("FadeOut");
        StartCoroutine("unableFade");
    }

    public void enableFadeIn()
    {
        fade.SetActive(true);
        fade.GetComponent<Animator>().Play("Fade");
        StartCoroutine("unableFade");
    }
    public void enableScreenFadeOut()
    {
        GameManager.GameState state = ServiceLocator.Get<GameManager>()._GameState;

        if (state == GameManager.GameState.Tutorial)
        {
            fadeTutorial.GetComponent<Animator>().Play("FadeOut");
        }
        else if (state == GameManager.GameState.GamePlay)
        {
            fadeDumster.GetComponent<Animator>().Play("FadeOut");
        }
    }
    public void enableScreenFadeIn()
    {
        GameManager.GameState state = ServiceLocator.Get<GameManager>()._GameState;

        if (state == GameManager.GameState.Tutorial)
        {
            fadeTutorial.SetActive(true);
            fadeTutorial.GetComponent<Animator>().Play("Fade");
            StartCoroutine(unableScreenFade());
        }
        else if (state == GameManager.GameState.GamePlay)
        {
            fadeDumster.SetActive(true);
            fadeDumster.GetComponent<Animator>().Play("Fade");
            StartCoroutine(unableScreenFade());
        }

    }

    private IEnumerator unableScreenFade()
    {
        yield return new WaitForSeconds(1.1f);
        ServiceLocator.Get<UIManager>().enableScreenFadeOut();
        yield return new WaitForSeconds(1.9f);

        GameManager.GameState state = ServiceLocator.Get<GameManager>()._GameState;

        if (state == GameManager.GameState.Tutorial)
        {
            fadeTutorial.SetActive(false);
        }
        else if (state == GameManager.GameState.GamePlay)
        {
            fadeDumster.SetActive(false);
        }
    }
    private IEnumerator unableFade()
    {
        yield return new WaitForSeconds(2.1f);
        fade.SetActive(false);
    }

    public UIManager Initialize()
    {
        waveTimerBar.fillAmount = 0.0f;

        restartButton.onClick.AddListener(ServiceLocator.Get<LevelManager>().Restart);
        continueButton.onClick.AddListener(ServiceLocator.Get<LevelManager>().PauseGame);
        mainmenuButton.onClick.AddListener(ServiceLocator.Get<LevelManager>().ReturnToMainMenu);
        pauseButton.onClick.AddListener(ServiceLocator.Get<LevelManager>().PauseGame);

        pauseScreen.SetActive(false);
        playerHealthBar.fillAmount = 0.0f;
        towerHealthPercentage.text = string.Empty;
        return this;
    }

    public void UpdateImage(DamageType type, float fill)
    {
        switch (type)
        {
            case DamageType.Poison:
                poisonCover.fillAmount = fill;
                break;
            case DamageType.Intimidate:
                intimidateCover.fillAmount = fill;
                break;
            case DamageType.Ultimate:
                ultCover.fillAmount = fill;
                break;
            case DamageType.Loading:
                loadingImg.fillAmount = fill;
                break;
            default:
                break;
        }
    }

    public IEnumerator Reset()
    {
        Debug.Log("Start Reset UI");

        timerObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        currentTimer = 0;
        ServiceLocator.Get<LevelManager>().PlayCancelled = false;
        waveTimerBar.fillAmount = 0.0f;


        player = ServiceLocator.Get<LevelManager>().playerInstance;
        tower = ServiceLocator.Get<LevelManager>().towerInstance;

        foreach (var obj in signifiersForWaves)
        {
            Destroy(obj);
        }

        signifiersForWaves.Clear();

        // Wave reset

        float time = 0.0f;

        RegisterSpawnTime[] registerSpawnTimes = FindObjectsOfType<RegisterSpawnTime>();
        foreach (RegisterSpawnTime registerSpawnTime in registerSpawnTimes)
        {
            if (maximumTimer < registerSpawnTime.MaximumSpawnTime)
                maximumTimer = registerSpawnTime.MaximumSpawnTime;
        }

        foreach (RegisterSpawnTime registerSpawnTime in registerSpawnTimes)
        {
            GameObject signifier = Instantiate(timerObject, timerObject.transform.position, timerObject.transform.rotation, canvas.transform) as GameObject;
            signifiersForWaves.Add(signifier);

            RectTransform rectTransform = waveTimerBar.GetComponent<RectTransform>();
            float barWidth = waveTimerBar.GetComponent<RectTransform>().rect.width;
            float xPos = Mathf.Lerp(rectTransform.position.x - barWidth, rectTransform.position.x + barWidth, registerSpawnTime.StartSpawnTime / maximumTimer);

            signifier.transform.position = new Vector3(xPos, waveTimerBar.transform.position.y + 20f);
        }

        waveTimerBar.fillAmount = 0.0f;
        timerObject.SetActive(false);
        // Animation Reset
        //AnimationTexture.SetBool("IsHit", false);
        //AnimationTexture.SetFloat("Energy", 0.0f);

        UpdatePlayerHealth(player.GetComponent<Player>().health, player.GetComponent<Player>()._maxHealth);
        UpdateTowerHealth(tower.GetComponent<Tower>().fullHealth);
        UpdateUltimatePercentage(player.GetComponent<Player>().UltimateCharge);

        StartTimer();
        yield return null;
    }

    public void StartTimer()
    {
        StartCoroutine(CountWaveTimer());
    }

    IEnumerator CountWaveTimer()
    {
        while (currentTimer < maximumTimer)
        {
            yield return new WaitForSeconds(1.0f);
            currentTimer += 1.0f;
            waveTimerBar.fillAmount = currentTimer / maximumTimer;
            if (ServiceLocator.Get<LevelManager>().PlayCancelled)
                break;
            yield return null;
        }
    }

    public void UpdatePlayerHealth(float curr, float max)
    {
        playerHealthBar.fillAmount = curr / max;
    }

    public void UpdateTowerHealth(float curr)
    {
        towerHealthPercentage.text = curr.ToString();
    }

    public void UpdateUltimatePercentage(float curr)
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (curr >= 80.0f)
        {
            IsPower = true;
            //PresentTextrue.GetComponent<RawImage>().texture = PowerFulTexture;
        }
        else
        {
            IsPower = false;
            //PresentTextrue.GetComponent<RawImage>().texture = BasicTexture;
        }

        //AnimationTexture.SetFloat("Energy", curr);
    }

    public IEnumerator HitAnimation()
    {
        if (!IsPower)
        {
            //AnimationTexture.SetBool("IsHit", true);
            //PresentTextrue.GetComponent<RawImage>().texture = SickTexture;
            yield return new WaitForSeconds(1.0f);
            //PresentTextrue.GetComponent<RawImage>().texture = BasicTexture;
            //AnimationTexture.SetBool("IsHit", false);
        }
        yield return null;
    }


}
