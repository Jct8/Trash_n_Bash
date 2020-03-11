using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private int levelToLoad = 1;
    private bool isClicked = false;
    private bool isLoadCutScene = false;
    public GameObject fadeScreen;

    void Start()
    {
        fadeScreen.SetActive(false);
        ServiceLocator.Get<GameManager>().changeGameState(GameManager.GameState.MainMenu);
    }
    public void OnLevelButtonClick(int level)
    {
        //if (isLoadCutScene)
        //{
        //    ServiceLocator.Get<GameManager>().sceneToLoad = level;
        //    SceneManager.LoadScene("CutScene");
        //}
        //else
        //{
            levelToLoad = level;
            StartCoroutine(LoadLevelRoutine());
        //}
    }

    public void LoadCutScene()
    {
        isLoadCutScene = true;
    }


    public void OnQuitClick()
    {
        Debug.Log("Quiting when in Build mode");
        Application.Quit();
    }

    private IEnumerator LoadLevelRoutine()
    {
        if (!isClicked)
        {
            fadeScreen.SetActive(true);
            fadeScreen.GetComponent<Animator>().Play("Fade");
            yield return new WaitForSeconds(1.0f);

            if (isLoadCutScene)
            {
                ServiceLocator.Get<GameManager>().sceneToLoad = levelToLoad;
                yield return SceneManager.LoadSceneAsync("CutScene");
            }
            else
            {
                yield return SceneManager.LoadSceneAsync(levelToLoad);
            }
            isClicked = true;
        }

    }
}
