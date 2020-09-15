using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInfoMenuMongo : MonoBehaviour
{
    public Button loginButton;
    public Button updateButton;
    public Button deleteButton;

    public Text playerLoginName;
    public GameObject loginHolder;
    public GameObject infoHolder;

    public InputField firstNameText;
    public InputField lastNameText;
    public InputField dobText;
    public InputField email;
    public InputField nickName;
    public Dropdown optInDropDown;

    public GameObject matchContent;
    public GameObject matchTextPrefab;

    private PlayerMongo currentPlayer;

    private void Awake()
    {
        loginHolder.SetActive(true);
        infoHolder.SetActive(false);
        loginButton.onClick.AddListener(LogIn);
        updateButton.onClick.AddListener(UpdatePlayer);
        deleteButton.onClick.AddListener(DeletePlayer);
    }

    public void BackButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LogIn()
    {
        var db = DatabaseConnection.Instance;
        string jsonResponse = db.GetAllPlayers();
        List<PlayerMongo> players = JsonConvert.DeserializeObject<List<PlayerMongo>>(jsonResponse);
        foreach (var player in players)
        {
            if (player.nickname == playerLoginName.text)
            {
                currentPlayer = player;
                loginHolder.SetActive(false);
                infoHolder.SetActive(true);
                DisplayPlayerInfo();
                DisplayMatches();
            }
        }
    }

    public void DeletePlayer()
    {
        // delete currentPlayer
        string jsonData = JsonUtility.ToJson(currentPlayer);
        DatabaseConnection.Instance.DeletePlayer(jsonData);
    }

    public void UpdatePlayer()
    {
        // Update currentPlayer with currentPlayer.Id from textfields
        string jsonData = JsonUtility.ToJson(currentPlayer);
        DatabaseConnection.Instance.UpdatePlayer(jsonData, currentPlayer.player_id);
    }

    void DisplayPlayerInfo()
    {
        if (currentPlayer != null)
        {
            firstNameText.text = currentPlayer.first_name;
            lastNameText.text = currentPlayer.last_name;
            dobText.text = currentPlayer.date_of_birth.ToShortDateString();
            nickName.text = currentPlayer.nickname;
            email.text = currentPlayer.email;
            optInDropDown.value = currentPlayer.opt_in == true ? 1 : 2;
        }
        else
        {
            firstNameText.text = "";
            lastNameText.text = "";
            dobText.text = "";
            email.text = "";
            optInDropDown.value = 0;
        }
    }

    void DisplayMatches()
    {
        if (currentPlayer!= null)
        {
            float counter = 0.5f;
            foreach (var match in currentPlayer.matches)
            {
                float size = matchTextPrefab.GetComponent<RectTransform>().rect.height;
                GameObject go = Instantiate(matchTextPrefab, matchContent.transform);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, -counter * size, go.transform.localPosition.z);
                counter += 1.0f;
                go.GetComponent<Text>().text = "level: "+ match.level_number.ToString() +" score:"+ match.score.ToString() +" date: "+ match.date.ToShortDateString();
            }
        }
    }
}
