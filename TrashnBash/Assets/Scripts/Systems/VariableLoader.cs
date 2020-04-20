using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetsToUnity;

public class VariableLoader : MonoBehaviour
{
    string associatedSheet = "10dcxJCHT_K4Jh-CG6FuRJ_b-7PIaWppBiqVVQgJ0_lQ";

    private Dictionary<string, float> RatStats = new Dictionary<string, float>();
    private Dictionary<string, float> CrowsStats = new Dictionary<string, float>();
    private Dictionary<string, float> OpossumsStats = new Dictionary<string, float>();
    private Dictionary<string, float> SkunkStats = new Dictionary<string, float>();

    private Dictionary<string, float> TowerStats = new Dictionary<string, float>();
    private Dictionary<string, float> PlayerStats = new Dictionary<string, float>();
    private Dictionary<string, Dictionary<string, float>> PlayerAbilties = new Dictionary<string, Dictionary<string, float>>();

    public bool useGoogleSheets = true;

    void Start()
    {
        if (!useGoogleSheets) return;

        SpreadsheetManager.Read(new GSTU_Search(associatedSheet, "Enemies"), UpdateEnemies);
        SpreadsheetManager.Read(new GSTU_Search(associatedSheet, "Stats", "A3", "B4", "A" , 3), UpdatePlayer);
        SpreadsheetManager.Read(new GSTU_Search(associatedSheet, "Stats", "A6", "D10", "A" , 6), UpdatePlayerAbilities);
    }

    void UpdatePlayer(GstuSpreadSheet ss)
    {
        float value;
        PlayerStats["HP"] = float.TryParse(ss["A4"].value, out value) == true ? value : 0.0f;
        PlayerStats["Speed"] = float.TryParse(ss["B4"].value, out value) == true ? value : 0.0f;
    }

    void UpdatePlayerAbilities(GstuSpreadSheet ss)
    {
        float value;
        PlayerAbilties["Attack"] = new Dictionary<string, float>();
        PlayerAbilties["Attack"]["Damage"] =   float.TryParse(ss["Basic Attack", "Damage"].value, out value) == true ? value : 0.0f;
        PlayerAbilties["Attack"]["Cooldown"] = float.TryParse(ss["Basic Attack", "Cooldown"].value, out value) == true ? value : 0.0f;
        PlayerAbilties["Attack"]["Range"] =    float.TryParse(ss["Basic Attack", "Range"].value, out value) == true ? value : 0.0f;

        PlayerAbilties["Poison"] = new Dictionary<string, float>();
        PlayerAbilties["Poison"]["Damage"] =   float.TryParse(ss["Fleas", "Damage"].value, out value) == true ? value : 0.0f;
        PlayerAbilties["Poison"]["Cooldown"] = float.TryParse(ss["Fleas", "Cooldown"].value, out value) == true ? value : 0.0f;
        PlayerAbilties["Poison"]["Range"] =    float.TryParse(ss["Fleas", "Range"].value, out value) == true ? value : 0.0f;

        PlayerAbilties["Stun"] = new Dictionary<string, float>();
        PlayerAbilties["Stun"]["Damage"] =   float.TryParse(ss["Stun", "Damage"].value, out value) == true ? value : 0.0f;
        PlayerAbilties["Stun"]["Cooldown"] = float.TryParse(ss["Stun", "Cooldown"].value, out value) == true ? value : 0.0f;
        PlayerAbilties["Stun"]["Range"] =    float.TryParse(ss["Stun", "Range"].value, out value) == true ? value : 0.0f;

        PlayerAbilties["Ultimate"] = new Dictionary<string, float>();
        PlayerAbilties["Ultimate"]["Damage"] =   float.TryParse(ss["Ultimate", "Damage"].value, out value) == true ? value : 0.0f;
        PlayerAbilties["Ultimate"]["Cooldown"] = float.TryParse(ss["Ultimate", "Cooldown"].value, out value) == true ? value : 0.0f;
        PlayerAbilties["Ultimate"]["Range"] =    float.TryParse(ss["Ultimate", "Range"].value, out value) == true ? value : 0.0f;
    }

    void UpdateTower(GstuSpreadSheet ss)
    {
        //health = int.Parse(ss[name, "Health"].value);
        //health = int.Parse(ss["C19"].value);
        // health = int.Parse(ss["Stun"].value);
    }

    void UpdateEnemies(GstuSpreadSheet ss)
    {
        float value;
        RatStats["HP"] =         float.TryParse(ss["Rats", "HP"].value, out value) == true ? value : 0.0f;
        RatStats["Speed"] =      float.TryParse(ss["Rats", "Speed"].value, out value) == true ? value : 0.0f;
        RatStats["Steal"] =      float.TryParse(ss["Rats", "Steal Amount"].value, out value) == true ? value : 0.0f;
        RatStats["Damage"] =     float.TryParse(ss["Rats", "Damage"].value, out value) == true ? value : 0.0f;
        RatStats["CoolDown"] =   float.TryParse(ss["Rats", "Cool downs"].value, out value) == true ? value : 0.0f;
        RatStats["Range"] =      float.TryParse(ss["Rats", "Range"].value, out value) == true ? value : 0.0f;
        RatStats["Power lvl"] =  float.TryParse(ss["Rats", "Power lvl"].value, out value) == true ? value : 0.0f;
        RatStats["w/ Ability"] = float.TryParse(ss["Rats", "w/ Ability"].value, out value) == true ? value : 0.0f;

        CrowsStats["HP"] =         float.TryParse(ss["Crows", "HP"].value, out value) == true ? value : 0.0f;
        CrowsStats["Speed"] =      float.TryParse(ss["Crows", "Speed"].value, out value) == true ? value : 0.0f;
        CrowsStats["Steal"] =      float.TryParse(ss["Crows", "Steal Amount"].value, out value) == true ? value : 0.0f;
        CrowsStats["Damage"] =     float.TryParse(ss["Crows", "Damage"].value, out value) == true ? value : 0.0f;
        CrowsStats["CoolDown"] =   float.TryParse(ss["Crows", "Cool downs"].value, out value) == true ? value : 0.0f;
        CrowsStats["Range"] =      float.TryParse(ss["Crows", "Range"].value, out value) == true ? value : 0.0f;
        CrowsStats["Power lvl"] =  float.TryParse(ss["Crows", "Power lvl"].value, out value) == true ? value : 0.0f;
        CrowsStats["w/ Ability"] = float.TryParse(ss["Crows", "w/ Ability"].value, out value) == true ? value : 0.0f;

        OpossumsStats["HP"] =         float.TryParse(ss["Opossums", "HP"].value, out value) == true ? value : 0.0f;
        OpossumsStats["Speed"] =      float.TryParse(ss["Opossums", "Speed"].value, out value) == true ? value : 0.0f;
        OpossumsStats["Steal"] =      float.TryParse(ss["Opossums", "Steal Amount"].value, out value) == true ? value : 0.0f;
        OpossumsStats["Damage"] =     float.TryParse(ss["Opossums", "Damage"].value, out value) == true ? value : 0.0f;
        OpossumsStats["CoolDown"] =   float.TryParse(ss["Opossums", "Cool downs"].value, out value) == true ? value : 0.0f;
        OpossumsStats["Range"] =      float.TryParse(ss["Opossums", "Range"].value, out value) == true ? value : 0.0f;
        OpossumsStats["Power lvl"] =  float.TryParse(ss["Opossums", "Power lvl"].value, out value) == true ? value : 0.0f;
        OpossumsStats["w/ Ability"] = float.TryParse(ss["Opossums", "w/ Ability"].value, out value) == true ? value : 0.0f;

        SkunkStats["HP"] =         float.TryParse(ss["Skunks", "HP"].value, out value) == true ? value : 0.0f;
        SkunkStats["Speed"] =      float.TryParse(ss["Skunks", "Speed"].value, out value) == true ? value : 0.0f;
        SkunkStats["Steal"] =      float.TryParse(ss["Skunks", "Steal Amount"].value, out value) == true ? value : 0.0f;
        SkunkStats["Damage"] =     float.TryParse(ss["Skunks", "Damage"].value, out value) == true ? value : 0.0f;
        SkunkStats["CoolDown"] =   float.TryParse(ss["Skunks", "Cool downs"].value, out value) == true ? value : 0.0f;
        SkunkStats["Range"] =      float.TryParse(ss["Skunks", "Range"].value, out value) == true ? value : 0.0f;
        SkunkStats["Power lvl"] =  float.TryParse(ss["Skunks", "Power lvl"].value, out value) == true ? value : 0.0f;
        SkunkStats["w/ Ability"] = float.TryParse(ss["Skunks", "w/ Ability"].value, out value) == true ? value : 0.0f;
    }
}
