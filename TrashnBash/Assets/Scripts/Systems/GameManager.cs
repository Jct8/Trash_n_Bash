using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameState _GameState { get; set; }

    public int videoNumbertoPlay = 1;
    public int currentlevel;
    public int highScore;
    private float coolTime = 10.0f;

    public float _houseHP = 50.0f;
    public float _racoonHP;
    public string sceneToLoad = "MainMenu";
    public bool _enemySkillActived { get; set; }

    public Dictionary<UpgradeMenu.Upgrade, int> upgradeLevelsDictionary = new Dictionary<UpgradeMenu.Upgrade, int>();
    public Dictionary<UpgradeMenu.Upgrade, bool> upgradeEnabled = new Dictionary<UpgradeMenu.Upgrade, bool>();
    public List<string> specialTargets = new List<string>();
    public string choosenTarget = "No Target";
    public BarricadeSpawner barricadeSpawner;

    private bool isVaribableLoaded = false;
    public enum GameState
    {
        Loader,
        MainMenu,
        GamePlay,
        Tutorial,
        GameWin,
        GameLose
    }

    private void Awake()
    {
        _GameState = GameState.Loader;
        specialTargets.Add("No Target");
        foreach (UpgradeMenu.Upgrade item in System.Enum.GetValues(typeof(UpgradeMenu.Upgrade)))
        {
            upgradeLevelsDictionary.Add(item, 0);
            upgradeEnabled.Add(item, false);
        }
    }

    public GameManager Initialize()
    {
        currentlevel = 0;
        return this;
    }

    private void Update()
    {
        switch (_GameState)
        {
            case GameState.Loader:
                ServiceLocator.Get<AudioManager>().gameObject.SetActive(true);
                break;
            case GameState.MainMenu:
                break;
            case GameState.GamePlay:
                if (ServiceLocator.Get<LevelManager>().CheckLoseCondition())
                {
                    _GameState = GameState.GameLose;
                    StartCoroutine(DelayEnding(3.0f));
                }
                else if (ServiceLocator.Get<LevelManager>().CheckWinCondition())
                {
                    _GameState = GameState.GameWin;
                    StartCoroutine(DelayEnding(3.0f));
                }
                break;
            case GameState.Tutorial:
                if (ServiceLocator.Get<LevelManager>().CheckLoseCondition())
                {
                    _GameState = GameState.GameLose;
                    StartCoroutine(DelayEnding(3.0f));
                }
                else if (ServiceLocator.Get<LevelManager>().CheckWinCondition())
                {
                    _GameState = GameState.GameWin;
                    StartCoroutine(DelayEnding(3.0f));
                }
                break;
        }
    }

    IEnumerator DelayEnding(float delay)
    {
        yield return new WaitForSeconds(delay);
        Time.timeScale = 0.0f;
        ServiceLocator.Get<UIManager>().endPanel.gameObject.SetActive(true);
        yield return null;
    }

    public void SetGameOver()
    {
        Time.timeScale = 1.0f;
        ServiceLocator.Get<UIManager>().endPanel.gameObject.SetActive(false);
        _GameState = GameState.MainMenu;
        SceneManager.LoadScene("GameOver");
    }

    public void SetGameWin()
    {
        Time.timeScale = 1.0f;
        ServiceLocator.Get<UIManager>().endPanel.gameObject.SetActive(false);

        Player player = ServiceLocator.Get<LevelManager>().playerInstance.GetComponent<Player>();
        Tower tower = ServiceLocator.Get<LevelManager>().towerInstance.GetComponent<Tower>();

        _racoonHP = player.health;
        _houseHP = tower.fullHealth;

        _GameState = GameState.MainMenu;
        currentlevel++;
        if (SceneManager.GetActiveScene().name == "Level4")
        {
            sceneToLoad = "MainMenu";
            videoNumbertoPlay = 2;
            SceneManager.LoadScene("CutScene");
        }
        else
        {
            SceneManager.LoadScene("UpgradeMenu");
        }
    }

    public void changeGameState(GameState state)
    {
        _GameState = state;
        if (_GameState == GameState.GamePlay || _GameState == GameState.Tutorial)
        {
            ServiceLocator.Get<UIManager>().gameObject.SetActive(true);
            ServiceLocator.Get<UIManager>().StartCoroutine(ServiceLocator.Get<UIManager>().Reset());
            ServiceLocator.Get<LevelManager>().gameObject.SetActive(true);
        }
    }

    public void enemySkillActived()
    {
        Debug.Log("Act!");
        _enemySkillActived = true;
        if(_enemySkillActived)
        {
            if (coolTime < Time.time)
            {
                coolTime = 10.0f + Time.time;
                _enemySkillActived = false;
                Debug.Log("Stop!");
            }
        }
    }

    public void LoadTowerHP()
    {
        if (isVaribableLoaded)
            return;
        VariableLoader variableLoader = ServiceLocator.Get<VariableLoader>();
        if (variableLoader.useGoogleSheets)
        {
            _houseHP = variableLoader.TowerStats["Health"];
        }
        isVaribableLoaded = true;
    }
}
