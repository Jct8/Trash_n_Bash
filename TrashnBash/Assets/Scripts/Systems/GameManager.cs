using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameState _GameState;
    public GameObject _AudioInstance;
    public GameObject _LevelInstance;

    public int _level;

    public enum GameState
    {
        Loader,
        MainMenu,
        GamePlay,
        GameOver
    }

    private void Awake()
    {
        _GameState = GameState.Loader;
    }

    public void UpdateGameState()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
            _GameState = GameState.MainMenu;
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            _GameState = GameState.GamePlay;
            _level = 1;
        }
        else
            return;
    }

    public GameManager Initialize()
    {
        _AudioInstance = new GameObject("AudioManager");
        _AudioInstance.SetActive(false);
        DontDestroyOnLoad(_AudioInstance);
       // AudioManager _AudioComp = _AudioInstance.AddComponent<AudioManager>();
        AudioManager _AudioComp = _AudioInstance.GetComponent<AudioManager>();
 
        ServiceLocator.Register<AudioManager>(_AudioComp);

        _LevelInstance = new GameObject("LevelManager");
        _LevelInstance.SetActive(false);
        DontDestroyOnLoad(_LevelInstance);
        LevelManager _LevelComp = _LevelInstance.AddComponent<LevelManager>();
        ServiceLocator.Register<LevelManager>(_LevelComp);

        return this;
    }

    private void Update()
    {
        UpdateGameState();
        switch(_GameState)
        {
            case GameState.Loader:
                break;
            case GameState.MainMenu:
                break;
            case GameState.GamePlay:
                _AudioInstance.SetActive(true);
                _LevelInstance.SetActive(true);
                _LevelInstance.GetComponent<LevelManager>().ResetLevel();
                break;
            case GameState.GameOver:
                break;
        }
    }

    public void SetGameOver()
    {

    }
}
