using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameState _GameState;

    public int currentlevel;
    public int highScore;

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
                StartCoroutine("SetGameWin");
                break;
            case GameState.GameLose:
                StartCoroutine("SetGameOver");
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
        _GameState = GameState.MainMenu;
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("GameWin");
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
            ServiceLocator.Get<LevelManager>().gameObject.SetActive(true);        }
    }
}
