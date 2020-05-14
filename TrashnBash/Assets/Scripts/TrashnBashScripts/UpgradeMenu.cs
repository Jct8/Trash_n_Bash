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
        BarricadeReductionCost,
        TargetEnemy,
        FireProjectile
    }

    Upgrade choosenUpgrade = Upgrade.None;

    public Button longRangedButton;
    public Button fireProjectileButton;
    public Button barricadeButton;
    public Button moreProjectilesButton;
    public Button targetEnemyButton;

    public Text upgradeCostText;
    public Text upgradeDescriptionText;
    public Text trashAvailableText;

    private UpgradeStats upgradeStats;
    private GameManager gameManager;

    int barricadeLevel;
    int extraProjectileLevel;
    int fireProjectileLevel;
    int longRangedLevel;
    int targetEnemyLevel;

    Button currentButton = null;
    ColorBlock colorBlock;

    private void Start()
    {
        gameManager = ServiceLocator.Get<GameManager>();
        barricadeLevel = gameManager.upgradeLevelsDictionary[Upgrade.BarricadeReductionCost];
        extraProjectileLevel = gameManager.upgradeLevelsDictionary[Upgrade.ExtraProjectiles];
        fireProjectileLevel = gameManager.upgradeLevelsDictionary[Upgrade.FireProjectile];
        longRangedLevel = gameManager.upgradeLevelsDictionary[Upgrade.Ranged];
        targetEnemyLevel = gameManager.upgradeLevelsDictionary[Upgrade.TargetEnemy];
        upgradeStats = ServiceLocator.Get<UpgradeStats>();
        trashAvailableText.text = "Trash Available:" + gameManager._houseHP.ToString();
    }

    public void Create()
    {
        if (choosenUpgrade != Upgrade.None)
        {
            int currentLevel = gameManager.upgradeLevelsDictionary[choosenUpgrade];
            switch (choosenUpgrade)
            {
                case Upgrade.ExtraProjectiles:
                    if (upgradeStats.targetEnmeyUpgradeCost.Count <= currentLevel + 1)
                    {
                        UpdateTrashCount(upgradeStats.moreProjectileCost[currentLevel]);
                        gameManager.upgradeLevelsDictionary[choosenUpgrade]++;
                    }
                    break;
                case Upgrade.Ranged:
                    if (upgradeStats.longRangedCost.Count >= currentLevel + 1)
                    {
                        UpdateTrashCount(upgradeStats.longRangedCost[currentLevel]);
                        gameManager.upgradeLevelsDictionary[choosenUpgrade]++;
                    }
                    break;
                case Upgrade.BarricadeReductionCost:
                    if (upgradeStats.baricadeUpgradeCost.Count >= currentLevel + 1)
                    {
                        UpdateTrashCount(upgradeStats.baricadeUpgradeCost[currentLevel]);
                        gameManager.upgradeLevelsDictionary[choosenUpgrade]++;
                    }
                    break;
                case Upgrade.TargetEnemy:
                    if (upgradeStats.targetEnmeyUpgradeCost.Count >= currentLevel + 1)
                    {
                        UpdateTrashCount(upgradeStats.targetEnmeyUpgradeCost[currentLevel]);
                        gameManager.upgradeLevelsDictionary[choosenUpgrade]++;
                    }
                    break;
                case Upgrade.FireProjectile:
                    if (upgradeStats.fireUpgradeCost.Count >= currentLevel + 1)
                    {
                        UpdateTrashCount(upgradeStats.fireUpgradeCost[currentLevel]);
                        gameManager.upgradeLevelsDictionary[choosenUpgrade]++;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void ChooseUpgrade(string upgrade)
    {
        switch (upgrade)
        {
            case "Barricades":
                choosenUpgrade = Upgrade.BarricadeReductionCost;
                ChangeColor(barricadeButton);
                upgradeDescriptionText.text = upgradeStats.barricadeDescription[barricadeLevel];
                upgradeCostText.text = "Trash Cost:" + upgradeStats.baricadeUpgradeCost[barricadeLevel].ToString();
                break;
            case "ExtraProjectiles":
                choosenUpgrade = Upgrade.ExtraProjectiles;
                ChangeColor(moreProjectilesButton);
                upgradeDescriptionText.text = upgradeStats.moreProjectileDescription[extraProjectileLevel];
                upgradeCostText.text = "Trash Cost:" + upgradeStats.moreProjectileCost[extraProjectileLevel].ToString();
                break;
            case "FireProjectile":
                choosenUpgrade = Upgrade.FireProjectile;
                ChangeColor(fireProjectileButton);
                upgradeDescriptionText.text = upgradeStats.fireDescription[fireProjectileLevel];
                upgradeCostText.text = "Trash Cost:" + upgradeStats.fireUpgradeCost[fireProjectileLevel].ToString();
                break;
            case "Ranged":
                choosenUpgrade = Upgrade.Ranged;
                ChangeColor(longRangedButton);
                upgradeDescriptionText.text = upgradeStats.longRangedDescription[longRangedLevel];
                upgradeCostText.text = "Trash Cost:" + upgradeStats.longRangedCost[longRangedLevel].ToString();
                break;
            case "TargetEnemy":
                choosenUpgrade = Upgrade.TargetEnemy;
                ChangeColor(targetEnemyButton);
                upgradeDescriptionText.text = upgradeStats.targetEnmeyDescription[targetEnemyLevel];
                upgradeCostText.text = "Trash Cost:" + upgradeStats.targetEnmeyUpgradeCost[targetEnemyLevel].ToString();
                break;
            default:
                break;
        }
    }

    public void BackButton()
    {
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

    private void UpdateTrashCount(float value)
    {
        gameManager._houseHP -= value;
        trashAvailableText.text = "Trash Available:" + gameManager._houseHP.ToString();
    }
}
