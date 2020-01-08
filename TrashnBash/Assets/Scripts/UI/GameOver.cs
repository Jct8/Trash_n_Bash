using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public AudioClip loseMusic;
    public Button returnButton;

    void Awake()
    {
        ServiceLocator.Get<AudioManager>().musicSource.Stop();
        ServiceLocator.Get<AudioManager>().musicSource.clip = loseMusic;
        ServiceLocator.Get<AudioManager>().musicSource.Play();
        ServiceLocator.Get<AudioManager>().musicSource.loop = false;
        returnButton.onClick.AddListener(ReturnToMainMenu);
    }

    void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
