﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Color gizmoColor = Color.red;

    [System.Serializable]
    public class EnemyPath
    {
        public Transform WayPointsHolder;
        public List<Transform> WayPoints { get; set; }

        public void SetupWaypoints()
        {
            WayPoints = new List<Transform>();
            foreach (Transform waypoint in WayPointsHolder)
                WayPoints.Add(waypoint);
        }
    }

    public GameObject _UnitPrefeb;

    // It's property for waves
    public int _numberOfWave;
    public int _enemiesPerWave;
    public int _secondBetweenWave;
    public int _secondStartDelay;
    public int _currentWave;
    public bool StartOnSceneLoad = true;

    public List<GameObject> _activeEnemies = new List<GameObject>();

    public EnemyPath _path;
    private Action OnRecycle;

    private void Awake()
    {
        _path.SetupWaypoints();

        if (_UnitPrefeb == null)
        {
            Debug.LogError("Enemy Spawner disabled: Unit Prefab is NULL");
            gameObject.SetActive(false);
            return;
        }
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

    public void ResetSpawner()
    {
        _currentWave = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        Gizmos.DrawWireCube(transform.position, Vector3.one);

        if (_path.WayPointsHolder == null)
            return;

        Gizmos.DrawLine(transform.position, _path.WayPointsHolder.GetChild(0).transform.position);
        int i = 0;
        foreach (Transform child in _path.WayPointsHolder)
        {
            if (i < _path.WayPointsHolder.childCount - 1)
            {
                Gizmos.DrawLine(child.transform.position, _path.WayPointsHolder.GetChild(i + 1).transform.position);
                Gizmos.DrawWireSphere(child.transform.position, 0.25f);
            }

            i++;
        }
    }
}
