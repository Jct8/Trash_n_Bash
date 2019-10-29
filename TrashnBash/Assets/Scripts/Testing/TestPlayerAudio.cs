using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerAudio : MonoBehaviour
{
    public AudioItemDefinition shootSFX;
    public AudioItemDefinition playerDeath;

    public AudioManager audioManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            audioManager.sfxSource.clip = shootSFX.clip;
            audioManager.sfxSource.Play();
        }
        else if (Input.GetKeyDown(KeyCode.F1))
        {
            audioManager.sfxSource.clip = playerDeath.clip;
            audioManager.sfxSource.Play();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            audioManager.FadeOutMusic();
        }
    }
}
