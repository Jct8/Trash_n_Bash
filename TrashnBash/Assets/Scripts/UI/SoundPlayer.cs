using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour, ICharacterSound
{
    public AudioClip Attack;
    public AudioClip TakeDamage;

    public AudioClip PoisonSound;
    public AudioClip PoisonTickSound;

    public AudioClip BarricadeTake;
    public AudioClip BarricadePlace;
    public AudioClip BarricadeBuild;

    private AudioSource source { get { return GetComponent<AudioSource>(); } }

    public IEnumerator PlaySound(int id)
    {
        switch (id)
        {
            case 0:
                {
                    yield return new WaitForSeconds(0.2f);
                    source.PlayOneShot(Attack);
                    break;
                }
            case 1:
                {
                    yield return new WaitForSeconds(0.2f);
                    source.PlayOneShot(TakeDamage);
                    break;
                }
            case 2:
                {
                    yield return new WaitForSeconds(0.2f);
                    source.PlayOneShot(PoisonSound);
                    break;
                }
            case 3:
                {
                    yield return new WaitForSeconds(0.2f);
                    source.PlayOneShot(PoisonTickSound);
                    break;
                }
            case 4:
                {
                    yield return new WaitForSeconds(0.2f);
                    source.PlayOneShot(BarricadeTake);
                    break;
                }
            case 5:
                {
                    yield return new WaitForSeconds(0.2f);
                    source.PlayOneShot(BarricadePlace);
                    break;
                }
            case 6:
                {
                    yield return new WaitForSeconds(0.2f);
                    source.PlayOneShot(BarricadeBuild);
                    break;
                }
            default:
                break;
        }


    }
}
