using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Topten : MonoBehaviour
{
    public List<GameObject> toptenPlayer = new List<GameObject>();
    public Dropdown durationDropDown;
    public void BackButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowTopTen()
    {
        ClearTopTen();
        MatchDuration duration = new MatchDuration();
        duration.FromDate = DateTime.MinValue;
        duration.ToDate = DateTime.MaxValue;

        switch (durationDropDown.value)
        {
            case 0: // All Time
                duration.FromDate = DateTime.MinValue;
                duration.ToDate = DateTime.MaxValue;
                break;
            case 1: // Last 2 years
                duration.FromDate = DateTime.Now.AddYears(-2);
                duration.ToDate = DateTime.Today;
                break;
            case 2: // Last Year
                duration.FromDate = DateTime.Now.AddYears(-1);
                duration.ToDate = DateTime.Today;
                break;
            case 3: // Last Month
                duration.FromDate = DateTime.Now.AddMonths(-1);
                duration.ToDate = DateTime.Today;
                break;
            case 4: // Last two weeks
                duration.FromDate = DateTime.Now.AddDays(-14);
                duration.ToDate = DateTime.Now;
                break;
            case 5:// Today
                duration.FromDate = DateTime.Today;
                duration.ToDate = DateTime.Now;
                break;
            default:
                break;
        }

        var db = DatabaseConnection.Instance;
        string jsonResponse = db.GetTopTenMatches(duration);
        List<TopTenMatch> matches;
        matches = JsonConvert.DeserializeObject<List<TopTenMatch>>(jsonResponse);

        int counter = 0;
        foreach (var match in matches)
        {
            GameObject name = toptenPlayer[counter].transform.Find("Name").gameObject.transform.Find("Text").gameObject;
            GameObject levelNum = toptenPlayer[counter].transform.Find("Level_Number").gameObject.transform.Find("Text").gameObject;
            GameObject date = toptenPlayer[counter].transform.Find("Date").gameObject.transform.Find("Text").gameObject;
            GameObject score = toptenPlayer[counter].transform.Find("HighScore").gameObject.transform.Find("Text").gameObject;

            name.GetComponent<Text>().text = match.player_nickname.ToString();
            levelNum.GetComponent<Text>().text = match.level_number.ToString();
            date.GetComponent<Text>().text = match.date.ToShortDateString();
            score.GetComponent<Text>().text = match.score.ToString();
            counter++;
        }
    }

    public void ClearTopTen()
    {
        foreach (var item in toptenPlayer)
        {
            GameObject name = item.transform.Find("Name").gameObject.transform.Find("Text").gameObject;
            GameObject levelNum = item.transform.Find("Level_Number").gameObject.transform.Find("Text").gameObject;
            GameObject date = item.transform.Find("Date").gameObject.transform.Find("Text").gameObject;
            GameObject score = item.transform.Find("HighScore").gameObject.transform.Find("Text").gameObject;

            name.GetComponent<Text>().text = "Null";
            levelNum.GetComponent<Text>().text = "Null";
            date.GetComponent<Text>().text = "Null";
            score.GetComponent<Text>().text = "Null";
        }
    }
}

