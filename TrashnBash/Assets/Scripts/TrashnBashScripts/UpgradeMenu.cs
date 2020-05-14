using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SheetCodes;

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
    private UpgradesModel upgradesModel;

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
        upgradesModel = ModelManager.UpgradesModel;
    }

    public void Create()
    {
        if (choosenUpgrade != Upgrade.None)
        {
            UpgradesIdentifier upgradesIdentifier;
            int currentLevel = gameManager.upgradeLevelsDictionary[choosenUpgrade];
            switch (choosenUpgrade)
            {
                case Upgrade.ExtraProjectiles:
                    upgradesIdentifier = upgradesModel.GetUpgradeEnum(Upgrade.ExtraProjectiles, currentLevel + 1);
                    //if (upgradeStats.targetEnmeyUpgradeCost.Count <= currentLevel + 1)
                    if (upgradesModel.GetTotalUpgrades(Upgrade.ExtraProjectiles) <= currentLevel + 2)
                    {
                        UpdateTrashCount(upgradesModel.GetRecord(upgradesIdentifier).TrashCost);//UpdateTrashCount(upgradeStats.moreProjectileCost[currentLevel]);
                        gameManager.upgradeLevelsDictionary[choosenUpgrade]++;
                    }
                    break;
                case Upgrade.Ranged:
                    upgradesIdentifier = upgradesModel.GetUpgradeEnum(Upgrade.Ranged, currentLevel + 1);
                    //if (upgradeStats.longRangedCost.Count >= currentLevel + 1)
                    if (upgradesModel.GetTotalUpgrades(Upgrade.Ranged) <= currentLevel + 2)
                    {
                        UpdateTrashCount(upgradesModel.GetRecord(upgradesIdentifier).TrashCost);//UpdateTrashCount(upgradeStats.longRangedCost[currentLevel]);
                        gameManager.upgradeLevelsDictionary[choosenUpgrade]++;
                    }
                    break;
                case Upgrade.BarricadeReductionCost:
                    upgradesIdentifier = upgradesModel.GetUpgradeEnum(Upgrade.BarricadeReductionCost, currentLevel + 1);
                    //if (upgradeStats.baricadeUpgradeCost.Count >= currentLevel + 1)
                    if (upgradesModel.GetTotalUpgrades(Upgrade.BarricadeReductionCost) <= currentLevel + 2)
                    {
                        UpdateTrashCount(upgradesModel.GetRecord(upgradesIdentifier).TrashCost);// UpdateTrashCount(upgradeStats.baricadeUpgradeCost[currentLevel]);
                        gameManager.upgradeLevelsDictionary[choosenUpgrade]++;
                    }
                    break;
                case Upgrade.TargetEnemy:
                    upgradesIdentifier = upgradesModel.GetUpgradeEnum(Upgrade.TargetEnemy, currentLevel + 1);
                    //if (upgradeStats.targetEnmeyUpgradeCost.Count >= currentLevel + 1)
                    if (upgradesModel.GetTotalUpgrades(Upgrade.TargetEnemy) <= currentLevel + 2)
                    {
                        UpdateTrashCount(upgradesModel.GetRecord(upgradesIdentifier).TrashCost);//UpdateTrashCount(upgradeStats.targetEnmeyUpgradeCost[currentLevel]);
                        gameManager.upgradeLevelsDictionary[choosenUpgrade]++;
                    }
                    break;
                case Upgrade.FireProjectile:
                    upgradesIdentifier = upgradesModel.GetUpgradeEnum(Upgrade.FireProjectile, currentLevel + 1);
                    //if (upgradeStats.fireUpgradeCost.Count >= currentLevel + 1)
                    if (upgradesModel.GetTotalUpgrades(Upgrade.FireProjectile) <= currentLevel + 2)
                    {
                        UpdateTrashCount(upgradesModel.GetRecord(upgradesIdentifier).TrashCost);//UpdateTrashCount(upgradeStats.fireUpgradeCost[currentLevel]);
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
        UpgradesIdentifier upgradesIdentifier;
        switch (upgrade)
        {
            case "Barricades":
                choosenUpgrade = Upgrade.BarricadeReductionCost;
                ChangeColor(barricadeButton);
                upgradesIdentifier = upgradesModel.GetUpgradeEnum(Upgrade.BarricadeReductionCost, barricadeLevel + 1);
                upgradeDescriptionText.text = upgradesModel.GetRecord(upgradesIdentifier).Description; // upgradeStats.barricadeDescription[barricadeLevel];
                upgradeCostText.text = "Trash Cost:" + upgradesModel.GetRecord(upgradesIdentifier).TrashCost;// upgradeStats.baricadeUpgradeCost[barricadeLevel].ToString();
                break;
            case "ExtraProjectiles":
                choosenUpgrade = Upgrade.ExtraProjectiles;
                ChangeColor(moreProjectilesButton);
                upgradesIdentifier = upgradesModel.GetUpgradeEnum(Upgrade.ExtraProjectiles, extraProjectileLevel + 1);
                upgradeDescriptionText.text = upgradesModel.GetRecord(upgradesIdentifier).Description;//upgradeStats.moreProjectileDescription[extraProjectileLevel];
                upgradeCostText.text = "Trash Cost:" + upgradesModel.GetRecord(upgradesIdentifier).TrashCost;// upgradeStats.moreProjectileCost[extraProjectileLevel].ToString();
                break;
            case "FireProjectile":
                choosenUpgrade = Upgrade.FireProjectile;
                ChangeColor(fireProjectileButton);
                upgradesIdentifier = upgradesModel.GetUpgradeEnum(Upgrade.FireProjectile, fireProjectileLevel + 1);
                upgradeDescriptionText.text = upgradesModel.GetRecord(upgradesIdentifier).Description; // upgradeStats.fireDescription[fireProjectileLevel];
                upgradeCostText.text = "Trash Cost:" + upgradesModel.GetRecord(upgradesIdentifier).TrashCost;// upgradeStats.fireUpgradeCost[fireProjectileLevel].ToString();
                break;
            case "Ranged":
                choosenUpgrade = Upgrade.Ranged;
                ChangeColor(longRangedButton);
                upgradesIdentifier = upgradesModel.GetUpgradeEnum(Upgrade.Ranged, longRangedLevel + 1);
                upgradeDescriptionText.text = upgradesModel.GetRecord(upgradesIdentifier).Description;// upgradeStats.longRangedDescription[longRangedLevel];
                upgradeCostText.text = "Trash Cost:" + upgradesModel.GetRecord(upgradesIdentifier).TrashCost;// upgradeStats.longRangedCost[longRangedLevel].ToString();
                break;
            case "TargetEnemy":
                choosenUpgrade = Upgrade.TargetEnemy;
                ChangeColor(targetEnemyButton);
                upgradesIdentifier = upgradesModel.GetUpgradeEnum(Upgrade.TargetEnemy, targetEnemyLevel + 1);
                upgradeDescriptionText.text = upgradesModel.GetRecord(upgradesIdentifier).Description; // upgradeStats.targetEnmeyDescription[targetEnemyLevel];
                upgradeCostText.text = "Trash Cost:" + upgradesModel.GetRecord(upgradesIdentifier).TrashCost;// upgradeStats.targetEnmeyUpgradeCost[targetEnemyLevel].ToString();
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
