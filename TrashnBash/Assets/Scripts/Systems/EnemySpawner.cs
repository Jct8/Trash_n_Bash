using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject _UnitPrefeb;

    // It's property for waves
    public int _numberOfWave;
    public int _enemiesPerWave;
    public int _secondBetweenWave;
    public int _secondStartDelay;
    public int _currentWave;
    public bool StartOnSceneLoad = true;

    public List<GameObject> _activeEnemies = new List<GameObject>();

    private WayPointManager.Path _path;
    public int _pathID;
    private Action OnRecycle;

    private void Awake()
    {
        if (_UnitPrefeb == null)
        {
            Debug.LogError("Enemy Spawner disabled: Unit Prefab is NULL");
            gameObject.SetActive(false);
            return;
        }
    }

    public void Init(WayPointManager.Path path)
    {
        _path = path;
    }

    public void StartSpawner()
    {
        StartCoroutine("BeginWaveSpawn");
    }

    private IEnumerator BeginWaveSpawn()
    {
        yield return new WaitForSeconds(_secondStartDelay);
        while (_currentWave < _numberOfWave)
        {
            SpawnWave(_currentWave);
            _currentWave++;
            yield return new WaitForSeconds(_secondBetweenWave);
        }
    }

    public void Recycle(GameObject obj)
    {
        ServiceLocator.Get<ObjectPoolManager>().RecycleObject(obj);
    }

    private void SpawnWave(int waveNumber)
    {
        for (int i = 0; i < _enemiesPerWave; i++)
        {
            GameObject _enemy = ServiceLocator.Get<ObjectPoolManager>().GetObjectFromPool(_UnitPrefeb.name);
            _enemy.transform.position = transform.position;
            _enemy.SetActive(true);
            OnRecycle = () => Recycle(_enemy);
            _enemy.GetComponent<Enemy>().Initialize(_path, OnRecycle);
            _enemy.GetComponent<Enemy>().Alive();
            _activeEnemies.Add(_enemy);
        }
    }
}
