using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioClip sound;

    public AudioClip buttonUp;
    public AudioClip buttonDown;

    void PlaySound()
    {
        AudioManager audioManager = ServiceLocator.Get<AudioManager>();
        audioManager?.PlaySfx(sound);
    }

    public void ButtonUP()
    {
        AudioManager audioManager = ServiceLocator.Get<AudioManager>();
        audioManager?.PlaySfx(buttonUp);
    }

    public void ButtonDown()
    {
        AudioManager audioManager = ServiceLocator.Get<AudioManager>();
        audioManager?.PlaySfx(buttonDown);
    }
}
