using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private int levelToLoad = 1;
    public void OnLevelButtonClick(int level)
    {
        levelToLoad = level;
        StartCoroutine(LoadLevelRoutine());
    }

    private IEnumerator LoadLevelRoutine()
    {
        yield return SceneManager.LoadSceneAsync(levelToLoad);
    }
}
