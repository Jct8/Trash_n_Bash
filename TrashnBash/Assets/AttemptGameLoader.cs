using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AttemptGameLoader : MonoBehaviour
{
    public GameLoader gameLoaderPrefab;
    public List<GameObject> prefabsToLoad;

    private void OnEnable()
    {
        if (!App.Instance.hasLoaded)
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            GameLoader gameLoader = Instantiate(gameLoaderPrefab);
            ServiceLocator.Register<GameLoader>(gameLoader);
            gameLoader.sceneToLoadIndex = buildIndex;
            SceneManager.UnloadSceneAsync(buildIndex);
        }
    }
}
