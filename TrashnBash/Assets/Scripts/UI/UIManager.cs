using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject player;
    public GameObject tower;
    public Slider playerHealthBar;
    public Slider waveTimerBar;
    public Text towerHealthPercentage;
    public Text ultimateChargePercentage;
    public Animator AnimationTexture;
    public Image EnergyBar;

    public GameObject PresentTextrue;
    public Texture BasicTexture;
    public Texture SickTexture;

    private float _TowerHP;
    private float fullEnergy = 100.0f;
    private float fullWaves = 25.0f;

    public UIManager Initialize()
    {
        playerHealthBar.value = 0.0f;
        waveTimerBar.maxValue = fullWaves;
        waveTimerBar.value = 25.0f;
        towerHealthPercentage.text = string.Empty;
        return this;
    }

    public IEnumerator Reset()
    {
        Debug.Log("Start Reset");
        yield return new WaitForSeconds(1.0f);
        player = GameObject.FindGameObjectWithTag("Player");
        tower = GameObject.FindGameObjectWithTag("Tower");

        waveTimerBar.value = fullWaves;
        AnimationTexture.SetBool("IsHit", false);
        UpdatePlayerHealth(player.GetComponent<Player>().Health);
        UpdateTowerHealth(tower.GetComponent<Tower>()._FullHealth);
        UpdateUltimatePercentage(player.GetComponent<Player>().UltimateCharge);
        StartCoroutine("CountingTimer");
        yield return null;
    }

    public IEnumerator CountingTimer()
    {
        yield return new WaitForSeconds(10.0f); // Wait for a wave
        while(waveTimerBar.value >= 0)
        {
            waveTimerBar.value--;
            yield return new WaitForSeconds(8.0f);
        }
        yield return null;
    }

    public void UpdatePlayerHealth(float curr)
    {
        playerHealthBar.value = curr;
    }

    public void UpdateTowerHealth(float curr)
    {
        towerHealthPercentage.text = curr.ToString() + " %";
    }

    public void UpdateUltimatePercentage(float curr)
    {
        ultimateChargePercentage.text = curr.ToString() + " %";
        EnergyBar.fillAmount = player.GetComponent<Player>()._ultimateCharge / fullEnergy;
    }

    public IEnumerator HitAnimation()
    {
        AnimationTexture.SetBool("IsHit", true);
        PresentTextrue.GetComponent<RawImage>().texture = SickTexture;
        yield return new WaitForSeconds(1.0f);
        PresentTextrue.GetComponent<RawImage>().texture = BasicTexture;
        AnimationTexture.SetBool("IsHit", false);
        yield return null;
    }
}
