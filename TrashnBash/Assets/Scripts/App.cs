using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class App: Singleton<App>
{
    public bool hasGameLoaded = false;

    public void LoadGameIntoScene(int sceneIndex)
    {
        StartCoroutine(LoadingGameIntoScene(sceneIndex));
    }

    private IEnumerator LoadingGameIntoScene(int sceneIndex)
    {
        SceneManager.LoadScene(0);
        yield return null;
        FindObjectOfType<GameLoader>().sceneToLoadIndex = sceneIndex;
    }
}
