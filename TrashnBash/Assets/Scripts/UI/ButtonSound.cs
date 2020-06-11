using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioClip sound;

    private Button button { get { return GetComponent<Button>(); } }
    void Start()
    {
        button.onClick.AddListener(() => PlaySound());
    }

    void PlaySound()
    {
        AudioManager audioManager = ServiceLocator.Get<AudioManager>();
        audioManager.PlaySfx(sound);
    }
}
