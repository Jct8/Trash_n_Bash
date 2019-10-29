using UnityEngine;
using UnityEngine.Audio;

public class AudioItemDefinition : ScriptableObject
{
    public AudioClip clip;
    public AudioMixerGroup mixerGroupTarget;
    public bool loop = false;
}
