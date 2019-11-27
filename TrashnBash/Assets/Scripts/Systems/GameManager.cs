using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameState _GameState;
    public GameObject _AudioInstance;
    public GameObject _LevelInstance;
    public GameObject _Player;
    public GameObject _Tower;

    public int _level;
    private bool _Start = false;
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

    public GameManager Initialize()
    {
        _AudioInstance = new GameObject("AudioManager");
        _AudioInstance.SetActive(false);
        DontDestroyOnLoad(_AudioInstance);
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
        switch (_GameState)
        {
            case GameState.Loader:
                break;
            case GameState.MainMenu:
                break;
            case GameState.GamePlay:
                if (!_Start)
                {
                    _AudioInstance.SetActive(true);
                    _LevelInstance.SetActive(true);
                    _LevelInstance.GetComponent<LevelManager>().ResetLevel();
                    ServiceLocator.Get<UIManager>().gameObject.SetActive(true);
                    ServiceLocator.Get<UIManager>().StartCoroutine("Reset");
                    _level = 1;
                    _Start = true;
                }
                else
                {
                    if(_Player == null && _Tower == null)
                    {
                        _Player = GameObject.FindGameObjectWithTag("Player");
                        _Tower = GameObject.FindGameObjectWithTag("Tower");
                    }
                    else
                    {
                        if (_Player.GetComponent<Player>().health <= 0.0f || _Tower.GetComponent<Tower>()._FullHealth <= 0.0f)
                        {
                            _GameState = GameState.GameOver;
                        }
                    }

                }
                break;
            case GameState.GameOver:
                break;
        }
    }

    public void SetGameOver()
    {

    }
}
