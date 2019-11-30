using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject player;
    public GameObject tower;
    public Slider playerHealthBar;
    public Text towerHealthPercentage;
    public Text ultimateChargePercentage;

    private float _TowerHP;

    public UIManager Initialize()
    {
        playerHealthBar.value = 0.0f;
        towerHealthPercentage.text = string.Empty;
        return this;
    }

    public IEnumerator Reset()
    {
        Debug.Log("Start Reset");
        yield return new WaitForSeconds(1.0f);
        player = GameObject.FindGameObjectWithTag("Player");
        tower = GameObject.FindGameObjectWithTag("Tower");

        UpdatePlayerHealth(player.GetComponent<Player>().Health);
        UpdateTowerHealth(tower.GetComponent<Tower>()._FullHealth);
        UpdateUltimatePercentage(player.GetComponent<Player>().UltimateCharge);
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
    }
}
