using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public Color gizmoColor = Color.red;

    [System.Serializable]
    public class EnemyPath
    {
        public List<Transform> WayPoints { get; set; }

        public void SetupWaypoints(Transform waypointsHolder)
        {
            WayPoints = new List<Transform>();
            foreach (Transform waypoint in waypointsHolder)
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

    private List<GameObject> _activeEnemies = new List<GameObject>();

    private EnemyPath _path;
    private Action OnRecycle;

    private void Awake()
    {
        _path = new EnemyPath();
        _path.SetupWaypoints(transform.GetChild(0));

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
            _enemy.transform.position = _path.WayPoints[0].transform.position;
            _enemy.SetActive(true);
            OnRecycle = () => Recycle(_enemy);
            _enemy.GetComponent<Enemy>().Initialize(_path, OnRecycle);
            _enemy.GetComponent<Enemy>().Alive();
            _activeEnemies.Add(_enemy);
        }
        ServiceLocator.Get<UIManager>().CountingTimer((float)_enemiesPerWave);
    }

    public void ResetSpawner()
    {
        _currentWave = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        Transform waypointsHolder = transform.GetChild(0);

        if (waypointsHolder == null)
            return;

        int i = 0;
        foreach (Transform child in waypointsHolder)
        {
            if (i < waypointsHolder.childCount - 1)
            {
                Gizmos.DrawLine(child.transform.position, waypointsHolder.GetChild(i + 1).transform.position);
                Gizmos.DrawSphere(child.transform.position, 0.25f);
            }

            i++;
        }
        Gizmos.DrawSphere(waypointsHolder.GetChild(waypointsHolder.childCount-1).transform.position, 0.25f);
    }
}
