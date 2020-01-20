using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private int levelToLoad = 1;
    void Start()
    {
        ServiceLocator.Get<GameManager>().changeGameState(GameManager.GameState.MainMenu);
    }
    public void OnLevelButtonClick(int level)
    {
        levelToLoad = level;
        StartCoroutine(LoadLevelRoutine());
    }

    public void OnQuitClick()
    {
        Debug.Log("Quiting when in Build mode");
        Application.Quit();
    }

    private IEnumerator LoadLevelRoutine()
    {
        ServiceLocator.Get<GameManager>().changeGameState(GameManager.GameState.GamePlay);
        yield return SceneManager.LoadSceneAsync(levelToLoad);
    }
}
