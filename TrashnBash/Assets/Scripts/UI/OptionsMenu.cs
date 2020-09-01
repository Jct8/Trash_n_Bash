using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    AudioManager audioManager;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    public GameObject gameUI;
    public GameObject pauseScreen;

    void Start()
    {
        audioManager = ServiceLocator.Get<AudioManager>();
        masterSlider?.onValueChanged.AddListener(audioManager.SetMasterVolume);
        musicSlider?.onValueChanged.AddListener(audioManager.SetMusicVolume);
        sfxSlider?.onValueChanged.AddListener(audioManager.SetSFXVolume);

        if(masterSlider) audioManager.SetMasterVolume(masterSlider.value);
        if(musicSlider) audioManager.SetMusicVolume(musicSlider.value);
        if(sfxSlider) audioManager.SetSFXVolume(sfxSlider.value);
    }

    public void ShowOptions()
    {
        if (ServiceLocator.Get<GameManager>()._GameState == GameManager.GameState.MainMenu)
        {
            ServiceLocator.Get<UIManager>().gameObject.SetActive(true);
            gameUI.SetActive(false);
        }
        pauseScreen.SetActive(false);
        gameObject.SetActive(true);
    }

    public void HideOptions()
    {
        if (ServiceLocator.Get<GameManager>()._GameState == GameManager.GameState.GamePlay ||
            ServiceLocator.Get<GameManager>()._GameState == GameManager.GameState.Tutorial)
        {
            pauseScreen.SetActive(true);
        }
        else if (ServiceLocator.Get<GameManager>()._GameState == GameManager.GameState.MainMenu)
        {
            ServiceLocator.Get<UIManager>().gameObject.SetActive(false);
            pauseScreen.SetActive(false);
            gameUI.SetActive(true);
        }
        gameObject.SetActive(false);
    }
}
