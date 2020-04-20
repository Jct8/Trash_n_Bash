using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetsToUnity;

public class TestGoogleSheets : MonoBehaviour
{
    string name = "Fly";
    int health = 2;

    string associatedSheet = "10dcxJCHT_K4Jh-CG6FuRJ_b-7PIaWppBiqVVQgJ0_lQ";
    //string associatedSheet = "1nfnS7cJd0gKanvorRqMhNcxCTkhTWh1szIZjCgwhKmg";
    string associatedWorksheet = "Stats";

    // Start is called before the first frame update
    void Start()
    {
        SpreadsheetManager.Read(new GSTU_Search(associatedSheet, associatedWorksheet,"A17","D21"), TestCallback);
    }

    void TestCallback(GstuSpreadSheet ss)
    {
        //health = int.Parse(ss[name, "Health"].value);
        health = int.Parse(ss["C19"].value);
       // health = int.Parse(ss["Stun"].value);
        Debug.Log(health);
    }
}
