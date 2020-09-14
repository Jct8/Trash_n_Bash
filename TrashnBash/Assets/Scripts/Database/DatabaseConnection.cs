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

    public PlayerSQL currentPlayer;

    public bool Login(string playerNickName)
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
                List<PlayerSQL> players = JsonConvert.DeserializeObject<List<PlayerSQL>>(jsonResponse);
                foreach (var player in players)
                {
                    if (player.nickname == playerNickName)
                    {
                        currentPlayer = player;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public List<MatchSQL> GetCurretPlayerMatches()
    {
        if (currentPlayer == null)
            return null;
        string uri = matchConnection + currentPlayer.player_id;
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
                List<MatchSQL> matches = JsonConvert.DeserializeObject<List<MatchSQL>>(jsonResponse);
                return matches;
            }
        }
        return null;
    }

}
