using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject _Prefeb;
    public GameObject _PlayerInstance;
    public GameObject _TowerInstance;

    public int _Level;
    public float _Money;
    public int _Score;

    private void Awake()
    {
        if (gameObject == null)
        {
            Debug.Log("Failed to find it, since it doesn't exist!");
        }
    }

    public void ClearLevel()
    {
        SaveData();
        ResetLevel();
        _Level++;
    }

    public void ResetLevel()
    {
        return;
    }

    public void SaveData()
    {
        return;

    }

    public void LoadData()
    {
        return;
    }
}
