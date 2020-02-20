using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
    public enum Upgrade
    {
        None,
        ExtraProjectiles,
        Ranged,
        Barricades,
        TargetEnemy,
        FireProjectile
    }

    Upgrade choosenUpgrade = Upgrade.None;

    public void Create()
    {
        if (choosenUpgrade != Upgrade.None)
        {
            ServiceLocator.Get<GameManager>().upgradeLevelsDictionary[choosenUpgrade]++;
        }
    }

    public void ChooseUpgrade(string upgrade)
    {
        switch (upgrade)
        {
            case "Barricades":
                choosenUpgrade = Upgrade.Barricades;
                break;
            case "ExtraProjectiles":
                choosenUpgrade = Upgrade.ExtraProjectiles;
                break;
            case "FireProjectile":
                choosenUpgrade = Upgrade.FireProjectile;
                break;
            case "Ranged":
                choosenUpgrade = Upgrade.Ranged;
                break;
            case "TargetEnemy":
                choosenUpgrade = Upgrade.TargetEnemy;
                break;
            default:
                break;
        }
    }
}
