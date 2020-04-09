using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelReset : MonoBehaviour
{
    public AudioClip BGM;

    private void Awake()
    {
        if (!App.Instance.hasGameLoaded)
            return;

        ServiceLocator.Get<LevelManager>().ResetLevel();
        ServiceLocator.Get<AudioManager>().musicSource.clip = BGM;
        ServiceLocator.Get<AudioManager>().musicSource.volume = 1.0f;
        ServiceLocator.Get<AudioManager>().musicSource.Play();
        ServiceLocator.Get<AudioManager>().musicSource.loop = true;
    }
}
