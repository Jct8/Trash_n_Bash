using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject player;
    public GameObject tower;
    public Image playerHealthBar;
    public Image waveTimerBar;
    public Text towerHealthPercentage;
    public Animator AnimationTexture;

    public GameObject PresentTextrue;
    public GameObject[] spawners;
    public GameObject pauseScreen;
    public Texture BasicTexture;
    public Texture SickTexture;
    public Texture PowerFulTexture;

    public Image loadingImg;
    public Image attackCover;
    public Image poisonCover;
    public Image intimidateCover;
    public Image ultCover;
    public Image repairIcon;
    public GameObject fade;
    public GameObject fadeDumster;


    public GameObject attackImg;
    public GameObject poisonImg;
    public GameObject intimidateImg;
    public GameObject ultImg;
    public GameObject repairButton;
    public GameObject placeButton;
    public GameObject basicAttackButton;

    public Button pauseButton;
    public Button continueButton;
    public Button restartButton;
    public Button mainmenuButton;

    private float _TowerHP;
    private float fullEnergy = 100.0f;
    private bool IsPower = false;
    public float totalWave = 0;
    public float currentWave = 0;
    private void Start()
    {
        fade.SetActive(false);
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
    public void enableDumsterFadeOut()
    {
        fadeDumster.GetComponent<Animator>().Play("FadeOut");
        //StartCoroutine(unableDumSterFade());
    }
    public void enableDumsterFadeIn()
    {
        fadeDumster.SetActive(true);
        fadeDumster.GetComponent<Animator>().Play("Fade");
        StartCoroutine(unableDumSterFade());
    }

    private IEnumerator unableDumSterFade()
    {
        yield return new WaitForSeconds(1.1f);
        ServiceLocator.Get<UIManager>().enableDumsterFadeOut();
        yield return new WaitForSeconds(1.9f);
        fadeDumster.SetActive(false);
    }
    private IEnumerator unableFade()
    {
        yield return new WaitForSeconds(2.1f);
        fade.SetActive(false);
    }

    public UIManager Initialize()
    {
        totalWave = 0;
        currentWave = 0;
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
            case DamageType.Normal:
                attackCover.fillAmount = fill;
                break;
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
        totalWave = 0.0f;
        currentWave = 0.0f;
        yield return new WaitForSeconds(0.5f);
        player = GameObject.FindGameObjectWithTag("Player");
        tower = GameObject.FindGameObjectWithTag("Tower");
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        foreach (GameObject spawn in spawners)
        {
            totalWave += spawn.GetComponent<EnemySpawner>()._numberOfWave;
        }
        currentWave = totalWave;
        waveTimerBar.fillAmount = currentWave / totalWave;
        AnimationTexture.SetBool("IsHit", false);
        AnimationTexture.SetFloat("Energy", 0.0f);
        UpdatePlayerHealth(player.GetComponent<Player>().health, player.GetComponent<Player>()._maxHealth);
        UpdateTowerHealth(tower.GetComponent<Tower>().fullHealth);
        UpdateUltimatePercentage(player.GetComponent<Player>().UltimateCharge);

        yield return null;
    }

    public void CountingTimer(float value)
    {
        currentWave -= value;
        waveTimerBar.fillAmount = currentWave / totalWave;
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
            PresentTextrue.GetComponent<RawImage>().texture = PowerFulTexture;
        }
        else
        {
            IsPower = false;
            PresentTextrue.GetComponent<RawImage>().texture = BasicTexture;
        }

        AnimationTexture.SetFloat("Energy", curr);
    }

    public IEnumerator HitAnimation()
    {
        if (!IsPower)
        {
            AnimationTexture.SetBool("IsHit", true);
            PresentTextrue.GetComponent<RawImage>().texture = SickTexture;
            yield return new WaitForSeconds(1.0f);
            PresentTextrue.GetComponent<RawImage>().texture = BasicTexture;
            AnimationTexture.SetBool("IsHit", false);
        }
        yield return null;
    }
}
