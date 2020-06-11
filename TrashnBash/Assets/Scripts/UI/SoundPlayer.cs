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

    public IEnumerator PlaySound(int id)
    {
        AudioManager audioManager = ServiceLocator.Get<AudioManager>();

        switch (id)
        {
            case 0:
                {
                    yield return new WaitForSeconds(0.2f);
                    audioManager.PlaySfx(Attack);
                    break;
                }
            case 1:
                {
                    yield return new WaitForSeconds(0.2f);
                    audioManager.PlaySfx(TakeDamage);
                    break;
                }
            case 2:
                {
                    yield return new WaitForSeconds(0.2f);
                    audioManager.PlaySfx(PoisonSound);
                    break;
                }
            case 3:
                {
                    yield return new WaitForSeconds(0.2f);
                    audioManager.PlaySfx(PoisonTickSound);
                    break;
                }
            case 4:
                {
                    yield return new WaitForSeconds(0.2f);
                    audioManager.PlaySfx(BarricadeTake);
                    break;
                }
            case 5:
                {
                    yield return new WaitForSeconds(0.2f);
                    audioManager.PlaySfx(BarricadePlace);
                    break;
                }
            case 6:
                {
                    yield return new WaitForSeconds(0.2f);
                    audioManager.PlaySfx(BarricadeBuild);
                    break;
                }
            default:
                break;
        }


    }
}
