using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour, IGameModule
{
    [Serializable]
    public class PooledObject
    {
        public string name;
        public GameObject prefab;
        public int poolSize;
    }
    public List<PooledObject> objectsToPool = new List<PooledObject>();
    private readonly Dictionary<string, List<GameObject>> _objectPoolByName = new Dictionary<string, List<GameObject>>();

    public bool IsInitialized { get { return _isInitialized; } }
    private bool _isInitialized = false;

    public IEnumerator LoadModule()
    {
        //Debug.Log("Initializing ObjectPool");
        InitializePool();
        yield return new WaitUntil(() => { return IsInitialized; });

        ServiceLocator.Register<ObjectPoolManager>(this);
    }

    private void InitializePool()
    {
        GameObject PoolManagerGO = new GameObject("Object Pool");
        PoolManagerGO.transform.SetParent(GameObject.FindWithTag("Services").transform);
        foreach (PooledObject poolObj in objectsToPool)
        {
            if (!_objectPoolByName.ContainsKey(poolObj.name))
            {
                GameObject poolGO = new GameObject(poolObj.name);
                poolGO.transform.SetParent(PoolManagerGO.transform);
                _objectPoolByName.Add(poolObj.name, new List<GameObject>());
                for(int i = 0; i < poolObj.poolSize; ++i)
                {
                    GameObject go = Instantiate(poolObj.prefab);
                    go.name = string.Format("{0}_{1:000}", poolObj.name, i);
                    go.transform.SetParent(poolGO.transform);
                    go.SetActive(false);
                    _objectPoolByName[poolObj.name].Add(go);
                }
            }
            else
            {
                Debug.Log("WARNING: Attempting to create multiple pools with the same name");
                continue;
            }
        }

        _isInitialized = true;
    }

    public GameObject GetObjectFromPool(string poolName)
    {
        GameObject ret = null;
        if (_objectPoolByName.ContainsKey(poolName))
        {
            ret = GetNextObject(poolName);
        }
        else
        {
            Debug.LogError($"No Pool Exists With Name: {poolName}");
        }
        return ret;
    }

    public List<GameObject> GetActiveObjects(string poolName)
    {
        List<GameObject> retList = new List<GameObject>();

        List<GameObject> pooledObjects = _objectPoolByName[poolName];
        foreach (GameObject go in pooledObjects)
        {
            if (go == null)
            {
                Debug.LogError("Pooled Object is NULL");
                continue;
            }

            if (go.activeInHierarchy)
            {
                retList.Add(go);
            }

        }

        return retList;
    }

    public List<string> GetKeys()
    {
        List<string> keyList = new List<string>(this._objectPoolByName.Keys);
        return keyList;
    }

    private GameObject GetNextObject(string poolName)
    {
        List<GameObject> pooledObjects = _objectPoolByName[poolName];
        foreach(GameObject go in pooledObjects)
        {
            if(go == null)
            {
                Debug.LogError("Pooled Object is NULL");
                continue;
            }

            if (go.activeInHierarchy)
            {
                continue;
            }
            else
            {
                return go;
            }
        }

        // TODO - Dynamic resize of object pool
        Debug.Log("Object Pool Depleted. No Unused Objects To Return");
        return null;
    }

    public void RecycleObject(GameObject go)
    {
        go.SetActive(false);
    }
}
