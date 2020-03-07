using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelLoader: MonoBehaviour
{
    private void Awake()
    {
        if (App.Instance.hasGameLoaded)
            return;

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"Invalid Scene Index {sceneIndex} ... Cannot load level from GameLoader.");
            return;
        }

        App.Instance.LoadGameIntoScene(sceneIndex);     
    }
}
