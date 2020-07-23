using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour, ICharacterSound
{
    public AudioClip Attack;
    public AudioClip TakeDamage;

    public AudioClip PoisonSound;
    public AudioClip PoisonTickSound2;
    public AudioClip PoisonTickSound;

    public AudioClip BarricadePlace;
    public AudioClip BarricadeBuild;

    public AudioClip PickingUpItem;
    public AudioClip RestoringHealth;

    public AudioClip TowerStealing;
    public AudioClip BarricadeDamage;
    public AudioClip BarricadeRepair;

    public IEnumerator BasicSound(int id)
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
                    int random = UnityEngine.Random.Range(1, 10);
                    if (random > 5)
                    {
                        audioManager.PlaySfx(PoisonTickSound);
                    }
                    else
                    {
                        audioManager.PlaySfx(PoisonTickSound2);
                    }
                    break;
                }
            case 4:
                {
                    yield return new WaitForSeconds(0.2f);
                    audioManager.PlaySfx(TowerStealing);
                    break;
                }
            case 5:
                {
                    yield return new WaitForSeconds(0.2f);
                    audioManager.PlaySfx(PickingUpItem);
                    break;
                }
            case 6:
                {
                    yield return new WaitForSeconds(0.2f);
                    audioManager.PlaySfx(RestoringHealth);
                    break;
                }
        }


    }

    public IEnumerator BarricadeSound(int id)
    {
        AudioManager audioManager = ServiceLocator.Get<AudioManager>();

        switch (id)
        {
            case 0:
                {
                    yield return new WaitForSeconds(0.2f);
                    audioManager.PlaySfx(BarricadePlace);
                    break;
                }
            case 1:
                {
                    yield return new WaitForSeconds(0.2f);
                    audioManager.PlaySfx(BarricadeBuild);
                    break;
                }
            case 2:
                {
                    yield return new WaitForSeconds(0.2f);
                    audioManager.PlaySfx(BarricadeDamage);
                    break;
                }
            case 3:
                {
                    yield return new WaitForSeconds(0.2f);
                    audioManager.PlaySfx(BarricadeRepair);
                    break;
                }
            default:
                break;
        }


    }
}
