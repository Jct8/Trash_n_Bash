using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject _Player;
    public GameObject _Tower;
    public Slider _PlayerHealthBar;
    public Text _TowerHealthPersentage;

    private float _TowerHP;

    public UIManager Initialize()
    {
        _PlayerHealthBar.value = 0.0f;
        _TowerHealthPersentage.text = string.Empty;
        return this;
    }

    public IEnumerator Reset()
    {
        Debug.Log("Start Reset");
        yield return new WaitForSeconds(1.0f);
        _Player = GameObject.FindGameObjectWithTag("Player");
        _Tower = GameObject.FindGameObjectWithTag("Tower");

        UpdatePlayerHealth(_Player.GetComponent<Player>().health);
        UpdateTowerHealth(_Tower.GetComponent<Tower>()._FullHealth);
        yield return null;
    }

    public void UpdatePlayerHealth(float curr)
    {
        _PlayerHealthBar.value = curr;
    }

    public void UpdateTowerHealth(float curr)
    {
        _TowerHealthPersentage.text = curr.ToString() + " %";
    }
}
