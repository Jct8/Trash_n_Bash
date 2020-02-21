using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameState _GameState;

    public int currentlevel;
    public int highScore;
    private float coolTime = 10.0f;

    public float _houseHP;
    public float _racoonHP;
    public bool _enemySkillActived { get; set; }

    public Dictionary<UpgradeMenu.Upgrade, int> upgradeLevelsDictionary = new Dictionary<UpgradeMenu.Upgrade, int>();

    public enum GameState
    {
        Loader,
        MainMenu,
        GamePlay,
        GameWin,
        GameLose
    }

    private void Awake()
    {
        _GameState = GameState.Loader;
        upgradeLevelsDictionary.Add(UpgradeMenu.Upgrade.Barricades, 1);
        upgradeLevelsDictionary.Add(UpgradeMenu.Upgrade.ExtraProjectiles, 1);
        upgradeLevelsDictionary.Add(UpgradeMenu.Upgrade.FireProjectile, 1);
        upgradeLevelsDictionary.Add(UpgradeMenu.Upgrade.Ranged, 1);
        upgradeLevelsDictionary.Add(UpgradeMenu.Upgrade.TargetEnemy, 1);
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
                break;
            case GameState.MainMenu:
                break;
            case GameState.GamePlay:
                if (ServiceLocator.Get<LevelManager>().CheckLoseCondition())
                {
                    _GameState = GameState.GameLose;
                }
                else if (ServiceLocator.Get<LevelManager>().CheckWinCondition())
                {
                    _GameState = GameState.GameWin;
                }
                break;
            case GameState.GameWin:
                StartCoroutine(SetGameWin());
                break;
            case GameState.GameLose:
                StartCoroutine(SetGameOver());
                break;
        }
    }

    public IEnumerator SetGameOver()
    {
        _GameState = GameState.MainMenu;
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("GameOver");
        yield return null;
    }

    public IEnumerator SetGameWin()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject tower = GameObject.FindGameObjectWithTag("Tower");

        _racoonHP = player.GetComponent<Player>().health;
        _houseHP = tower.GetComponent<Tower>().fullHealth;

        _GameState = GameState.MainMenu;
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("UpgradeMenu");
        yield return null;
    }

    public void changeGameState(GameState state)
    {
        _GameState = state;
        if (_GameState == GameState.GamePlay)
        {
            ServiceLocator.Get<UIManager>().gameObject.SetActive(true);
            ServiceLocator.Get<UIManager>().StartCoroutine("Reset");
            ServiceLocator.Get<AudioManager>().gameObject.SetActive(true);
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
}
