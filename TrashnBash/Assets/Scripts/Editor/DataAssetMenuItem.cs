using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class ScriptableObjectUtility
{
    public static void CreateAsset<T>() where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();
        ProjectWindowUtil.CreateAsset(asset, "New " + typeof(T).Name + ".asset");
    }
}

public class DataAssetMenuItem 
{
    [MenuItem("Assets/Create/Data Source/Json Data Source")]
    public static void CreateRawTextDataSource()
    {
        ScriptableObjectUtility.CreateAsset<JsonDataSource>();
    }

    [MenuItem("Assets/Create/Audio/AudioItem")]
    public static void CreateAudioItemDefinition()
    {
        ScriptableObjectUtility.CreateAsset<AudioItemDefinition>();
    }
}
