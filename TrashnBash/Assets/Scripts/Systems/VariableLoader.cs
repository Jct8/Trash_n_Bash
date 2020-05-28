using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetsToUnity;

public class VariableLoader : MonoBehaviour
{
    string associatedSheet = "10dcxJCHT_K4Jh-CG6FuRJ_b-7PIaWppBiqVVQgJ0_lQ";

    public Dictionary<string, Dictionary<string, float>> EnemyStats = new Dictionary<string, Dictionary<string, float>>();

    public Dictionary<string, float> TowerStats = new Dictionary<string, float>();
    public Dictionary<string, float> PickUpStats = new Dictionary<string, float>();
    public Dictionary<string, float> PlayerStats = new Dictionary<string, float>();
    public Dictionary<string, float> BarriacdeStats = new Dictionary<string, float>();
    public Dictionary<string, float> TrashCanStats = new Dictionary<string, float>();

    public Dictionary<string, Dictionary<string, float>> PlayerAbilties = new Dictionary<string, Dictionary<string, float>>();
    public Dictionary<string, Dictionary<string, float>> TowerUpgrades = new Dictionary<string, Dictionary<string, float>>();

    public bool useGoogleSheets = true;

    void Awake()
    {
        if (!useGoogleSheets) return;

        SpreadsheetManager.ReadPublicSpreadsheet(new GSTU_Search(associatedSheet, "Enemies"), UpdateEnemies);

        SpreadsheetManager.ReadPublicSpreadsheet(new GSTU_Search(associatedSheet, "Stats", "A3", "B4", "A" , 3), UpdatePlayer);
        SpreadsheetManager.ReadPublicSpreadsheet(new GSTU_Search(associatedSheet, "Stats", "A6", "D10", "A" , 6), UpdatePlayerAbilities);

        SpreadsheetManager.ReadPublicSpreadsheet(new GSTU_Search(associatedSheet, "Trash", "A2", "A3", "A", 2), UpdateTower);
        SpreadsheetManager.ReadPublicSpreadsheet(new GSTU_Search(associatedSheet, "Trash", "A27", "C28", "A", 27), UpdateTower2);
        SpreadsheetManager.ReadPublicSpreadsheet(new GSTU_Search(associatedSheet, "Trash", "A27", "C28", "A", 27), UpdatePickUp);

        SpreadsheetManager.ReadPublicSpreadsheet(new GSTU_Search(associatedSheet, "Trash", "A6", "H16", "A", 6), UpdateTowerUpgrades);
        SpreadsheetManager.ReadPublicSpreadsheet(new GSTU_Search(associatedSheet, "Trash", "A23", "B24", "A", 23), UpdateTrashCans);
        SpreadsheetManager.ReadPublicSpreadsheet(new GSTU_Search(associatedSheet, "Trash", "A19", "C20", "A", 19), UpdateBarricades);
    }

    public VariableLoader Initialize()
    {
        return this;
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
        PlayerAbilties["Poison"]["Range"] =    float.TryParse(ss["Fleas", "Range"].value.Split(' ')[0], out value) == true ? value : 0.0f;

        PlayerAbilties["Stun"] = new Dictionary<string, float>();
        PlayerAbilties["Stun"]["Damage"] =   float.TryParse(ss["Stun", "Damage"].value, out value) == true ? value : 0.0f;
        PlayerAbilties["Stun"]["Cooldown"] = float.TryParse(ss["Stun", "Cooldown"].value, out value) == true ? value : 0.0f;
        PlayerAbilties["Stun"]["Range"] =    float.TryParse(ss["Stun", "Range"].value, out value) == true ? value : 0.0f;

        PlayerAbilties["Ultimate"] = new Dictionary<string, float>();
        PlayerAbilties["Ultimate"]["Damage"] =   float.TryParse(ss["Ultimate", "Damage"].value, out value) == true ? value : 0.0f;
        PlayerAbilties["Ultimate"]["Cooldown"] = float.TryParse(ss["Ultimate", "Cooldown"].value, out value) == true ? value : 0.0f;
        PlayerAbilties["Ultimate"]["Range"] =    float.TryParse(ss["Ultimate", "Range"].value.Split(' ')[0], out value) == true ? value : 0.0f;
    }

    void UpdateEnemies(GstuSpreadSheet ss)
    {
        float value;
        EnemyStats["Rats"] = new Dictionary<string, float>();
        EnemyStats["Rats"]["HP"] =         float.TryParse(ss["Rats", "HP"].value, out value) == true ? value : 0.0f;
        EnemyStats["Rats"]["Speed"] =      float.TryParse(ss["Rats", "Speed"].value, out value) == true ? value : 0.0f;
        EnemyStats["Rats"]["Steal"] =      float.TryParse(ss["Rats", "Steal Amount"].value, out value) == true ? value : 0.0f;
        EnemyStats["Rats"]["Damage"] =     float.TryParse(ss["Rats", "Damage"].value, out value) == true ? value : 0.0f;
        EnemyStats["Rats"]["CoolDown"] =   float.TryParse(ss["Rats", "Cool downs"].value, out value) == true ? value : 0.0f;
        EnemyStats["Rats"]["Range"] =      float.TryParse(ss["Rats", "Range"].value, out value) == true ? value : 0.0f;
        EnemyStats["Rats"]["Power lvl"] =  float.TryParse(ss["Rats", "Power lvl"].value, out value) == true ? value : 0.0f;
        EnemyStats["Rats"]["w/ Ability"] = float.TryParse(ss["Rats", "w/ Ability"].value, out value) == true ? value : 0.0f;

        EnemyStats["Crows"] = new Dictionary<string, float>();
        EnemyStats["Crows"]["HP"] =         float.TryParse(ss["Crows", "HP"].value, out value) == true ? value : 0.0f;
        EnemyStats["Crows"]["Speed"] =      float.TryParse(ss["Crows", "Speed"].value, out value) == true ? value : 0.0f;
        EnemyStats["Crows"]["Steal"] =      float.TryParse(ss["Crows", "Steal Amount"].value, out value) == true ? value : 0.0f;
        EnemyStats["Crows"]["Damage"] =     float.TryParse(ss["Crows", "Damage"].value, out value) == true ? value : 0.0f;
        EnemyStats["Crows"]["CoolDown"] =   float.TryParse(ss["Crows", "Cool downs"].value, out value) == true ? value : 0.0f;
        EnemyStats["Crows"]["Range"] =      float.TryParse(ss["Crows", "Range"].value, out value) == true ? value : 0.0f;
        EnemyStats["Crows"]["Power lvl"] =  float.TryParse(ss["Crows", "Power lvl"].value, out value) == true ? value : 0.0f;
        EnemyStats["Crows"]["w/ Ability"] = float.TryParse(ss["Crows", "w/ Ability"].value, out value) == true ? value : 0.0f;

        EnemyStats["Opossums"] = new Dictionary<string, float>();
        EnemyStats["Opossums"]["HP"] =         float.TryParse(ss["Opossums", "HP"].value, out value) == true ? value : 0.0f;
        EnemyStats["Opossums"]["Speed"] =      float.TryParse(ss["Opossums", "Speed"].value, out value) == true ? value : 0.0f;
        EnemyStats["Opossums"]["Steal"] =      float.TryParse(ss["Opossums", "Steal Amount"].value, out value) == true ? value : 0.0f;
        EnemyStats["Opossums"]["Damage"] =     float.TryParse(ss["Opossums", "Damage"].value, out value) == true ? value : 0.0f;
        EnemyStats["Opossums"]["CoolDown"] =   float.TryParse(ss["Opossums", "Cool downs"].value, out value) == true ? value : 0.0f;
        EnemyStats["Opossums"]["Range"] =      float.TryParse(ss["Opossums", "Range"].value, out value) == true ? value : 0.0f;
        EnemyStats["Opossums"]["Power lvl"] =  float.TryParse(ss["Opossums", "Power lvl"].value, out value) == true ? value : 0.0f;
        EnemyStats["Opossums"]["w/ Ability"] = float.TryParse(ss["Opossums", "w/ Ability"].value, out value) == true ? value : 0.0f;

        EnemyStats["Skunks"] = new Dictionary<string, float>();
        EnemyStats["Skunks"] ["HP"] =         float.TryParse(ss["Skunks", "HP"].value, out value) == true ? value : 0.0f;
        EnemyStats["Skunks"] ["Speed"] =      float.TryParse(ss["Skunks", "Speed"].value, out value) == true ? value : 0.0f;
        EnemyStats["Skunks"] ["Steal"] =      float.TryParse(ss["Skunks", "Steal Amount"].value, out value) == true ? value : 0.0f;
        EnemyStats["Skunks"] ["Damage"] =     float.TryParse(ss["Skunks", "Damage"].value, out value) == true ? value : 0.0f;
        EnemyStats["Skunks"] ["CoolDown"] =   float.TryParse(ss["Skunks", "Cool downs"].value, out value) == true ? value : 0.0f;
        EnemyStats["Skunks"] ["Range"] =      float.TryParse(ss["Skunks", "Range"].value, out value) == true ? value : 0.0f;
        EnemyStats["Skunks"] ["Power lvl"] =  float.TryParse(ss["Skunks", "Power lvl"].value, out value) == true ? value : 0.0f;
        EnemyStats["Skunks"]["w/ Ability"] = float.TryParse(ss["Skunks", "w/ Ability"].value, out value) == true ? value : 0.0f;
    }

    void UpdateTower(GstuSpreadSheet ss)
    {
        float value;
        TowerStats["Health"] = float.TryParse(ss["A3"].value, out value) == true ? value : 0.0f;
    }
    void UpdateTower2(GstuSpreadSheet ss)
    {
        float value;
        TowerStats["PlayerHeal"] = float.TryParse(ss["A28"].value, out value) == true ? value : 0.0f;
        TowerStats["TrashCost"] = float.TryParse(ss["B28"].value, out value) == true ? value : 0.0f;
    }

    void UpdatePickUp(GstuSpreadSheet ss)
    {
        float value;
        PickUpStats["HealAmount"] = float.TryParse(ss["C28"].value, out value) == true ? value : 0.0f;
    }

    void UpdateTowerUpgrades(GstuSpreadSheet ss)
    {
        float value;
        TowerUpgrades["Basic"] = new Dictionary<string, float>();
        TowerUpgrades["Basic"]["Damage"] =   float.TryParse(ss["Basic", "Damage"].value, out value) == true ? value : 0.0f;
        TowerUpgrades["Basic"]["Cooldown"] = float.TryParse(ss["Basic", "Cooldown"].value, out value) == true ? value : 0.0f;
        TowerUpgrades["Basic"]["Range"] =    float.TryParse(ss["Basic", "Range"].value, out value) == true ? value : 0.0f;

        TowerUpgrades["Fire"] = new Dictionary<string, float>();
        TowerUpgrades["Fire"]["Damage"] =    float.TryParse(ss["Fire", "Damage"].value, out value) == true ? value : 0.0f;
        TowerUpgrades["Fire"]["Cooldown"] =  float.TryParse(ss["Fire", "Cooldown"].value, out value) == true ? value : 0.0f;
        TowerUpgrades["Fire"]["Range"] =     float.TryParse(ss["Fire", "Range"].value, out value) == true ? value : 0.0f;

        TowerUpgrades["LongRange"] = new Dictionary<string, float>();
        TowerUpgrades["LongRange"]["Damage"] =   float.TryParse(ss["Long Range", "Damage"].value, out value) == true ? value : 0.0f;
        TowerUpgrades["LongRange"]["Cooldown"] = float.TryParse(ss["Long Range", "Cooldown"].value, out value) == true ? value : 0.0f;
        TowerUpgrades["LongRange"]["Range"] =    float.TryParse(ss["Long Range", "Range"].value, out value) == true ? value : 0.0f;

        TowerUpgrades["DoubleProjectiles"] = new Dictionary<string, float>();
        TowerUpgrades["DoubleProjectiles"]["Damage"] =   float.TryParse(ss["Double Projectiles", "Damage"].value, out value) == true ? value : 0.0f;
        TowerUpgrades["DoubleProjectiles"]["Cooldown"] = float.TryParse(ss["Double Projectiles", "Cooldown"].value, out value) == true ? value : 0.0f;
        TowerUpgrades["DoubleProjectiles"]["Range"] =    float.TryParse(ss["Double Projectiles", "Range"].value, out value) == true ? value : 0.0f;
    }

    void UpdateTrashCans(GstuSpreadSheet ss)
    {
        float value;
        TrashCanStats["AmountCollected"] = float.TryParse(ss["A24"].value, out value) == true ? value : 0.0f;
        //TrashCanStats["TrashLimit"] = float.TryParse(ss["B18"].value, out value) == true ? value : 0.0f;
        TrashCanStats["Cooldown"] = float.TryParse(ss["B24"].value, out value) == true ? value : 0.0f;
    }
    void UpdateBarricades(GstuSpreadSheet ss)
    {
        float value;
        BarriacdeStats["Health"] = float.TryParse(ss["A20"].value, out value) == true ? value : 0.0f;
        BarriacdeStats["TrashCost"] = float.TryParse(ss["B20"].value, out value) == true ? value : 0.0f;
        BarriacdeStats["CooldownTime"] = float.TryParse(ss["C20"].value, out value) == true ? value : 0.0f;
    }
}
