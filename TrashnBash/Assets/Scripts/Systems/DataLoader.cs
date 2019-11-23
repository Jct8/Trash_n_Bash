using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader : MonoBehaviour, IGameModule
{
    public List<UnityEngine.Object> DataSources;
    public Dictionary<string, IDataSource> LoadedDataSources;

    private void Awake()
    {
        LoadedDataSources = new Dictionary<string, IDataSource>();
        GameLoader.CallOnComplete(Init);
    }

    private void Init()
    {
        ServiceLocator.Register<DataLoader>(this);
    }

    public IEnumerator LoadModule()
    {
        foreach(var obj in DataSources)
        {
            if(obj is IDataSource)
            {
                IDataSource source = (IDataSource)obj;
                yield return LoadAsync(source);
            }
        }

        yield return null;
    }

    public IEnumerator LoadAsync(IDataSource source)
    {
        if (!source.IsLoading)
        {
            source.IsLoading = true;
            yield return source.LoadAsync();
            source.IsLoaded = true;
            LoadedDataSources.Add(source.Id, source);
            //Debug.Log("Loaded Source: " + source.Id);
        }

        yield return null;
    }

    public IDataSource GetDataSourceById(string id)
    {
        if (LoadedDataSources.ContainsKey(id))
        {
            return LoadedDataSources[id];
        }
        return null;
    }
}
