using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    public Button longRangedButton;
    public Button fireProjectileButton;
    public Button barricadeButton;
    public Button moreProjectilesButton;
    public Button targetEnemyButton;

    Button currentButton = null;

    ColorBlock colorBlock;

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
                ChangeColor(barricadeButton);
                break;
            case "ExtraProjectiles":
                choosenUpgrade = Upgrade.ExtraProjectiles;
                ChangeColor(moreProjectilesButton);
                break;
            case "FireProjectile":
                choosenUpgrade = Upgrade.FireProjectile;
                ChangeColor(fireProjectileButton);
                break;
            case "Ranged":
                choosenUpgrade = Upgrade.Ranged;
                ChangeColor(longRangedButton);
                break;
            case "TargetEnemy":
                choosenUpgrade = Upgrade.TargetEnemy;
                ChangeColor(targetEnemyButton);
                break;
            default:
                break;
        }
    }

    public void BackButton()
    {
        //ServiceLocator.Get<LevelManager>().ResetLevel();
        SceneManager.LoadScene("MainMenu"); 
    }

    private void ChangeColor(Button button)
    {
        if (currentButton != null)
        {
            colorBlock = currentButton.colors;
            colorBlock.normalColor = new Color(Color.white.r, Color.white.g, Color.white.b);
            currentButton.colors = colorBlock;
        }

        currentButton = button;
        colorBlock = currentButton.colors;
        colorBlock.normalColor = colorBlock.selectedColor;
        currentButton.colors = colorBlock;

    }
}
