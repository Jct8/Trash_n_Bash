using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Topten : MonoBehaviour
{
    public void BackButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
    //public List<GameObject> toptenPlayer = new List<GameObject>();
    //public GameObject toptenInfo;

    //void EnableList()
    //{
    //    // Load Top ten players from MongoDB
    //    for (int i = 0; i < JsonData; i++)
    //    {
    //        toptenPlayer.Add(GameObject);
    //    }

    //    // Sorting who has highest scores in descending
    //    toptenPlayer.Sort(delegate(GameObject a, GameObject b)
    //    {
    //        if (a.GetComponent<info>().highScore > b.GetComponent<info>().highScore)
    //            return 1;
    //        else
    //            return -1;
    //    });

    //    // Listing
    //    float yAxis = 120.0f;
    //    for (int i = 0; i < 10; i++)
    //    {
    //        GameObject info = Instantiate(toptenInfo, new Vector3(0.0f, yAxis, 0.0f), Quaternion.identity);
    //        Texting(ref info,i);
    //        yAxis -= 20.0f;
    //    }
    //}

    //private void Texting(ref GameObject infomation, int PlayerId)
    //{
    //    foreach(GameObject Obj in infomation.transform)
    //    {
    //        switch(Obj.name)
    //        {
    //            case "Name":
    //                {
    //                    Obj.transform.gameObject.GetComponent<TextMesh>().text = toptenInfo[PlayerId].GetComponent<info>().name;
    //                    break;
    //                }
    //            case "Level_Number":
    //                {
    //                    Obj.transform.gameObject.GetComponent<TextMesh>().text = toptenInfo[PlayerId].GetComponent<info>().level_number;
    //                    break;
    //                }
    //            case "HighScore":
    //                {
    //                    Obj.transform.gameObject.GetComponent<TextMesh>().text = toptenInfo[PlayerId].GetComponent<info>().highScore;
    //                    break;
    //                }
    //            case "Date":
    //                {
    //                    Obj.transform.gameObject.GetComponent<TextMesh>().text = toptenInfo[PlayerId].GetComponent<info>().date;
    //                    break;
    //                }
    //            default:
    //                break;
    //        }
    //    }
    //}
}
