using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SheetCodes;
using System.Linq;

public class UpgradeMenu : MonoBehaviour
{
    public enum Upgrade
    {
        None,
        ExtraProjectiles,
        Ranged,
        BarricadeReductionCost,
        TargetEnemy,
        FireProjectile,

        ImprovedBarricades,
        BarricadeSpawnRate,
        TrashSpawnRate,
        ImprovedPlayerHP,
        ImprovedHealing
    }

    Upgrade choosenUpgrade = Upgrade.None;

    public float minimumTrashToSpend = 15.0f;

    public Button longRangedButton;
    public Button fireProjectileButton;
    public Button barricadeButton;
    public Button moreProjectilesButton;
    public Button targetEnemyButton;

    public Button improvedBarricadeBtn;
    public Button barricadeSpawnBtn;
    public Button trashSpawnBtn;
    public Button hpBtn;
    public Button healingBtn;

    public Text upgradeCostText;
    public Text upgradeDescriptionText;
    public Text trashAvailableText;

    public Text messageText;

    private GameManager gameManager;
    private UpgradesModel upgradesModel;

    int barricadeLevel;
    int extraProjectileLevel;
    int fireProjectileLevel;
    int longRangedLevel;
    int targetEnemyLevel;

    int improvedBarricadeLevel;
    int barricadeSpawnLevel;
    int trashSpawnLevel;
    int hpLevel;
    int healingLevel;

    Button currentButton = null;
    ColorBlock colorBlock;

    private void Start()
    {
        gameManager = ServiceLocator.Get<GameManager>();
        // Update Levels
        barricadeLevel = gameManager.upgradeLevelsDictionary[Upgrade.BarricadeReductionCost];
        extraProjectileLevel = gameManager.upgradeLevelsDictionary[Upgrade.ExtraProjectiles];
        fireProjectileLevel = gameManager.upgradeLevelsDictionary[Upgrade.FireProjectile];
        longRangedLevel = gameManager.upgradeLevelsDictionary[Upgrade.Ranged];
        targetEnemyLevel = gameManager.upgradeLevelsDictionary[Upgrade.TargetEnemy];

        improvedBarricadeLevel = gameManager.upgradeLevelsDictionary[Upgrade.ImprovedBarricades];
        barricadeSpawnLevel = gameManager.upgradeLevelsDictionary[Upgrade.BarricadeSpawnRate];
        trashSpawnLevel = gameManager.upgradeLevelsDictionary[Upgrade.TrashSpawnRate];
        hpLevel = gameManager.upgradeLevelsDictionary[Upgrade.ImprovedPlayerHP];
        healingLevel = gameManager.upgradeLevelsDictionary[Upgrade.ImprovedHealing];

        trashAvailableText.text = "Trash Available:" + gameManager._houseHP.ToString();
        upgradesModel = ModelManager.UpgradesModel;

        UpdateUI();
    }

    public void Create()
    {
        if (choosenUpgrade != Upgrade.None)
        {
            int currentLevel = gameManager.upgradeLevelsDictionary[choosenUpgrade];
            ApplyUpgrade(choosenUpgrade, currentLevel);
        }
    }

    public void ChooseUpgrade(string upgrade)
    {
        switch (upgrade)
        {
            case "Barricades":
                choosenUpgrade = Upgrade.BarricadeReductionCost;
                barricadeLevel = gameManager.upgradeLevelsDictionary[choosenUpgrade];
                ChangeColor(barricadeButton);
                DisplayChoosenUpgrade(choosenUpgrade, barricadeLevel);
                break;
            case "ExtraProjectiles":
                choosenUpgrade = Upgrade.ExtraProjectiles;
                extraProjectileLevel = gameManager.upgradeLevelsDictionary[choosenUpgrade];
                ChangeColor(moreProjectilesButton);
                DisplayChoosenUpgrade(choosenUpgrade, extraProjectileLevel);
                break;
            case "FireProjectile":
                choosenUpgrade = Upgrade.FireProjectile;
                fireProjectileLevel = gameManager.upgradeLevelsDictionary[choosenUpgrade];
                ChangeColor(fireProjectileButton);
                DisplayChoosenUpgrade(choosenUpgrade, fireProjectileLevel);
                break;
            case "Ranged":
                choosenUpgrade = Upgrade.Ranged;
                longRangedLevel = gameManager.upgradeLevelsDictionary[choosenUpgrade];
                ChangeColor(longRangedButton);
                DisplayChoosenUpgrade(choosenUpgrade, longRangedLevel);
                break;
            case "TargetEnemy":
                choosenUpgrade = Upgrade.TargetEnemy;
                longRangedLevel = gameManager.upgradeLevelsDictionary[choosenUpgrade];
                ChangeColor(targetEnemyButton);
                DisplayChoosenUpgrade(choosenUpgrade, longRangedLevel);
                break;
            case "ImprovedBarricades":
                choosenUpgrade = Upgrade.ImprovedBarricades;
                improvedBarricadeLevel = gameManager.upgradeLevelsDictionary[choosenUpgrade];
                ChangeColor(improvedBarricadeBtn);
                DisplayChoosenUpgrade(choosenUpgrade, improvedBarricadeLevel);
                break;
            case "BarricadeSpawnRate":
                choosenUpgrade = Upgrade.BarricadeSpawnRate;
                barricadeSpawnLevel = gameManager.upgradeLevelsDictionary[choosenUpgrade];
                ChangeColor(barricadeSpawnBtn);
                DisplayChoosenUpgrade(choosenUpgrade, barricadeSpawnLevel);
                break;
            case "TrashSpawnRate":
                choosenUpgrade = Upgrade.TrashSpawnRate;
                trashSpawnLevel = gameManager.upgradeLevelsDictionary[choosenUpgrade];
                ChangeColor(trashSpawnBtn);
                DisplayChoosenUpgrade(choosenUpgrade, trashSpawnLevel);
                break;
            case "ImprovedPlayerHP":
                choosenUpgrade = Upgrade.ImprovedPlayerHP;
                hpLevel = gameManager.upgradeLevelsDictionary[choosenUpgrade];
                ChangeColor(hpBtn);
                DisplayChoosenUpgrade(choosenUpgrade, hpLevel);
                break;
            case "ImprovedHealing":
                choosenUpgrade = Upgrade.ImprovedHealing;
                healingLevel = gameManager.upgradeLevelsDictionary[choosenUpgrade];
                ChangeColor(healingBtn);
                DisplayChoosenUpgrade(choosenUpgrade, healingLevel);
                break;
            default:
                break;
        }
    }

    public void BackButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    #region HelperFunctions

    private void UpdateUI()
    {
        // Update Button Text
        UpdateButtonText(barricadeButton, Upgrade.BarricadeReductionCost);
        UpdateButtonText(longRangedButton, Upgrade.Ranged);
        UpdateButtonText(fireProjectileButton, Upgrade.FireProjectile);
        UpdateButtonText(targetEnemyButton, Upgrade.TargetEnemy);
        UpdateButtonText(moreProjectilesButton, Upgrade.ExtraProjectiles);

        UpdateButtonText(improvedBarricadeBtn, Upgrade.ImprovedBarricades);
        UpdateButtonText(barricadeSpawnBtn, Upgrade.BarricadeSpawnRate);
        UpdateButtonText(trashSpawnBtn, Upgrade.TrashSpawnRate);
        UpdateButtonText(hpBtn, Upgrade.ImprovedPlayerHP);
        UpdateButtonText(healingBtn, Upgrade.ImprovedHealing);
    }

    private void UpdateButtonText(Button button, Upgrade upgrade)
    {
        int currentLevel = gameManager.upgradeLevelsDictionary[upgrade];
        UpgradesIdentifier upgradesIdentifier = upgradesModel.GetUpgradeEnum(upgrade, currentLevel + 1);

        if (upgradesIdentifier == UpgradesIdentifier.None)
            upgradesIdentifier = upgradesModel.GetUpgradeEnum(upgrade, currentLevel);
        var enumType = typeof(UpgradesIdentifier);
        var memberInfos = enumType.GetMember(upgradesIdentifier.ToString());
        var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
        var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(Identifier), false);
        var description = ((Identifier)valueAttributes[0]).enumIdentifier;

        int maxUpgradeLevel = upgradesModel.GetTotalUpgrades(upgrade);
        //if (maxUpgradeLevel == currentLevel) // Check if upgrade is max
        //    button.GetComponentInChildren<Text>().text = description + "\n - Max Level.";
        //else
        //    button.GetComponentInChildren<Text>().text = description;
    }

    private void ApplyUpgrade(Upgrade upgrade, int currentLevel)
    {
        UpgradesIdentifier upgradesIdentifier = upgradesModel.GetUpgradeEnum(upgrade, currentLevel + 1);
        int maxUpgradeLevel = upgradesModel.GetTotalUpgrades(upgrade);
        if (maxUpgradeLevel > currentLevel)
        {
            if (gameManager._houseHP - upgradesModel.GetRecord(upgradesIdentifier).TrashCost <= minimumTrashToSpend) // Cannot afford upgrade
            {
                messageText.text = "Cannot Upgrade. Your trash cannot be less than "+ minimumTrashToSpend+ " after the upgrade.";
                messageText.transform.parent.gameObject.SetActive(true);
                return;
            }

            UpdateTrashCount(upgradesModel.GetRecord(upgradesIdentifier).TrashCost);
            gameManager.upgradeLevelsDictionary[upgrade]++;
            gameManager.upgradeEnabled[upgrade] = true;
            currentLevel++;
            if (upgrade == Upgrade.TargetEnemy) // Add target to list
            {
                gameManager.specialTargets.Add(upgradesModel.GetRecord(upgradesIdentifier).Target);
                gameManager.choosenTarget = upgradesModel.GetRecord(upgradesIdentifier).Target;
            }

            //Update UI
            if (maxUpgradeLevel > currentLevel) // Another upgrade available
            {
                upgradesIdentifier = upgradesModel.GetUpgradeEnum(upgrade, currentLevel + 1);
                DisplayChoosenUpgrade(upgrade, currentLevel);
            }
            UpdateUI();
        }
        else
        {
            messageText.text = "Maxium Upgrade Already Equiped.";
            messageText.transform.parent.gameObject.SetActive(true);
        }
    }

    private void DisplayChoosenUpgrade(Upgrade upgrade, int level)
    {
        UpgradesIdentifier upgradesIdentifier = upgradesModel.GetUpgradeEnum(upgrade, level + 1);
        if (upgradesIdentifier == UpgradesIdentifier.None)
            upgradesIdentifier = upgradesModel.GetUpgradeEnum(upgrade, level);

        upgradeDescriptionText.text = upgradesModel.GetRecord(upgradesIdentifier).Description;
        upgradeCostText.text = "Trash Cost:" + upgradesModel.GetRecord(upgradesIdentifier).TrashCost;
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

    #endregion

}
