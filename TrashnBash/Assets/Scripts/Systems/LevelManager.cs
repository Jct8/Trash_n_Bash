using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject _Prefeb;
    public GameObject _PlayerInstance;
    public GameObject _TowerInstance;

    public float _SavedDamage;
    public float _SavedHealth;

    public float _SavedTowerDamage;
    public float _SavedTowerSpeed;
    public float _SavedTowerHealth;
    public float _SavedTowerAttackRate;
    public float _SavedTowerRange;
    public int _Level;
    //public float _Money;

    private void Awake()
    {
        if(gameObject == null)
        {
            Debug.Log("Failed to find it, since it doesn't exist!");
        }
    }

    public void ClearLevel()
    {
        SavePlayerData();
        ResetLevel();
        _Level++;
    }

    public void ResetLevel()
    {

        _PlayerInstance = GameObject.FindGameObjectWithTag("Player");
        _TowerInstance = GameObject.FindGameObjectWithTag("Tower");
        if(SceneManager.GetActiveScene().buildIndex >= 3)
        {
            _TowerInstance.GetComponent<Tower>().Initialize(_SavedTowerDamage, _SavedTowerSpeed, _SavedTowerHealth, _SavedTowerAttackRate, _SavedTowerRange);
            _PlayerInstance.GetComponent<Player>().Initialize(_SavedDamage, _SavedHealth);
        }
    }

    public void SavePlayerData()
    {
        _SavedDamage = ServiceLocator.Get<Player>().attack;
        _SavedHealth = ServiceLocator.Get<Player>().health;

        _SavedTowerDamage = ServiceLocator.Get<Tower>()._damage;
        _SavedTowerSpeed = ServiceLocator.Get<Tower>()._speed;
        _SavedTowerHealth = ServiceLocator.Get<Tower>()._health;
        _SavedTowerAttackRate = ServiceLocator.Get<Tower>()._attackRate;
        _SavedTowerRange = ServiceLocator.Get<Tower>()._range;

    }

    public void LoadPlayerData()
    {

    }
}
