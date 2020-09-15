using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DatabaseConnection : MonoBehaviour
{
    // Singleton
    private static DatabaseConnection instance;
    public static DatabaseConnection Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<DatabaseConnection>();

                if (instance == null)
                {
                    GameObject container = new GameObject("DatabaseConnection");
                    instance = container.AddComponent<DatabaseConnection>();
                    DontDestroyOnLoad(container);
                }
            }
            return instance;
        }
    }

    private string playerConnection = "http://localhost:5000/api/Player/";
    private string matchConnection = "http://localhost:5000/api/Match/";

    public string GetAllPlayers()
    {
        string uri = playerConnection;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.SendWebRequest();
            while (!webRequest.isDone){}
            if (webRequest.error != null)
                Debug.Log(uri + " Error: " + webRequest.error);
            else
            {
                //Debug.Log(uri + " Received: " + webRequest.downloadHandler.text);
                //Debug.Log(jsonResponse);
                string jsonResponse = webRequest.downloadHandler.text;
                return jsonResponse;
            }
        }
        return null;
    }

    public string GetCurretPlayerMatches<T>(T playerId)
    {
        string uri = matchConnection + playerId;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.SendWebRequest();
            while (!webRequest.isDone) { }
            if (webRequest.error != null)
                Debug.Log(uri + " Error: " + webRequest.error);
            else
            {
                //Debug.Log(uri + " Received: " + webRequest.downloadHandler.text);
                //Debug.Log(jsonResponse);
                string jsonResponse = webRequest.downloadHandler.text;
                return jsonResponse;
            }
        }
        return null;
    }

    public void UpdatePlayer<T>(string jsonData, T playerId)
    {
        
    }

    public void DeletePlayer<T>(T playerId)
    {

    }

    public string GetTopTenMatches<T>(MatchDuration matchDuration)
    {
        return null;
    }
}
