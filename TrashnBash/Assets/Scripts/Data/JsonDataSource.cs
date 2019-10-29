using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JsonFx;

public class JsonDataSource : ScriptableObject, IDataSource
{
    #region IDataSource Implementation
    public string Id { get; set; }
    public bool IsLoading { get; set; }
    public bool IsLoaded { get; set; }
    public string LoadError { get; set; }
    public DateTime LoadedTime { get; set; }

    public void Load()
    {
    }
    #endregion

    public Action OnLoaded;
    public TextAsset JsonTextAsset;
    public Dictionary<string, object> DataDictionary;

    public void OnEnable()
    {
        LoadError = string.Empty;
        LoadedTime = System.DateTime.UtcNow;
        DataDictionary = null;
        IsLoaded = false;
        IsLoading = false;
        Id = string.Empty;
    }

    public IEnumerator LoadAsync()
    {
        yield return new WaitForEndOfFrame();
        try
        {
            object deserializedObject = JsonFx.Json.JsonReader.Deserialize(JsonTextAsset.text);
            if (deserializedObject is Dictionary<string, object>)
            {
                DataDictionary = (Dictionary<string, object>)deserializedObject;
                LoadedTime = System.DateTime.UtcNow;
                Id = JsonTextAsset.name;
                IsLoaded = true;
            }
            else
            {
                this.LoadError = "TextAsset does not deserialize correctly. Check your JSON format";
            }
        }
        catch(System.Exception e)
        {
            LoadError = $"Exception occurred while trying to parse json: {e.Message}";
        }

        yield return new WaitForEndOfFrame();
    }

    public void HandleOnLoaded()
    {
        OnLoaded?.Invoke();
    }
}
