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
        string uri = playerConnection + playerId;
        using (UnityWebRequest webRequest = new UnityWebRequest(uri, "PUT"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SendWebRequest();
            while (!webRequest.isDone) { }
            if (webRequest.isNetworkError)
            {
                Debug.Log(uri + " Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(uri + " Received: " + webRequest.downloadHandler.text);
            }
        }

    }

    public void DeletePlayer<T>(T playerId)
    {
        string uri = playerConnection + playerId;
        UnityWebRequest webRequest = UnityWebRequest.Delete(uri);
        webRequest.SendWebRequest();
        while (!webRequest.isDone) { }
        if (webRequest.isNetworkError)
        {
            Debug.Log(uri + " Error: " + webRequest.error);
        }
        else
        {
            Debug.Log(uri + " Deleted");
        }
    }

    public string GetTopTenMatches<T>(MatchDuration matchDuration)
    {
        string uri = playerConnection + "FromDate=" + matchDuration.FromDate + "ToData=" + matchDuration.ToDate;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.SendWebRequest();
            while (!webRequest.isDone) { }
            if(webRequest.isNetworkError)
            {
                Debug.Log(uri + "Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(uri + " Received: " + webRequest.downloadHandler.text);

                MatchMongo matchesMongo = JsonUtility.FromJson<MatchMongo>("{\"matches\":" + webRequest.downloadHandler.text + "}");
                MatchSQL matchesSQL = JsonUtility.FromJson<MatchSQL>("{\"matches\":" + webRequest.downloadHandler.text + "}");

                foreach(var match in matchesMongo)
                {

                }
            }
        }

            return null;
    }
}
