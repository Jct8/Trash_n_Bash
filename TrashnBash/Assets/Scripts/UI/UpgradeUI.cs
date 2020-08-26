using SheetCodes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Upgrade = UpgradeMenu.Upgrade;

public class UpgradeUI : MonoBehaviour
{
    [System.Serializable]
    public class SpriteMapping
    {
        public Upgrade upgradeType;
        public Sprite sprite;
    }

    public List<SpriteMapping> buttonSprites = new List<SpriteMapping>();
    private Dictionary<Upgrade, Sprite> buttonSpritesDictionary = new Dictionary<Upgrade, Sprite>();

    public GameObject upgradeExample;
    public Transform upgradeHolder;
    public Text upgradeDescriptionText;

    private GameManager gameManager;
    private UpgradesModel upgradesModel;

    Dictionary<Upgrade, GameObject> listOfUpgrades = new Dictionary<Upgrade, GameObject>();
    int counter = 0;

    private void OnEnable()
    {
        foreach (var item in buttonSprites)
        {
            if (!buttonSpritesDictionary.ContainsKey(item.upgradeType))
                buttonSpritesDictionary.Add(item.upgradeType, item.sprite);
        }
        upgradesModel = ModelManager.UpgradesModel;
        gameManager = ServiceLocator.Get<GameManager>();
        foreach (var upgrade in gameManager.upgradeLevelsDictionary)
        {
            if (upgrade.Value > 0 && !listOfUpgrades.ContainsKey(upgrade.Key))
            {
                GameObject go = Instantiate(upgradeExample, upgradeHolder) as GameObject;
                float size = upgradeExample.GetComponent<RectTransform>().rect.height;
                go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y - counter * size, go.transform.position.z);

                // Update button text
                Button button = go.GetComponentInChildren<Button>();
                //UpdateButtonText(button, upgrade.Key);
                Image buttonImage = button.gameObject.GetComponent<Image>();
                buttonImage.sprite = buttonSpritesDictionary[upgrade.Key];
                buttonImage.type = Image.Type.Simple;
                buttonImage.preserveAspect = true;

                button.onClick.AddListener(() => DisplayChoosenUpgrade(upgrade.Key));

                // Update Drop Down
                Dropdown dropdown = go.GetComponentInChildren<Dropdown>();
                if (upgrade.Key == Upgrade.TargetEnemy)
                {
                    dropdown.AddOptions(gameManager.specialTargets);
                    if (gameManager.choosenTarget != "No Target")
                    {
                        int index = gameManager.specialTargets.IndexOf(gameManager.choosenTarget);
                        dropdown.value = index;
                    }
                }
                else
                    dropdown.gameObject.SetActive(false);

                // Update Toggle enable
                Toggle toggle = go.GetComponentInChildren<Toggle>();
                if (upgrade.Key != Upgrade.ImprovedPlayerHP)
                {
                    toggle.isOn = gameManager.upgradeEnabled[upgrade.Key];
                }
                else
                    toggle.gameObject.SetActive(false);

                counter++;
                listOfUpgrades.Add(upgrade.Key, go);
            }
            else if (listOfUpgrades.ContainsKey(upgrade.Key) && upgrade.Key == Upgrade.TargetEnemy)
            {
                GameObject go = listOfUpgrades[upgrade.Key];
                Dropdown dropdownTarget = go.GetComponentInChildren<Dropdown>();
                dropdownTarget.ClearOptions();
                dropdownTarget.AddOptions(gameManager.specialTargets);
                if (gameManager.choosenTarget != "No Target")
                {
                    int index = gameManager.specialTargets.IndexOf(gameManager.choosenTarget);
                    dropdownTarget.value = index;
                }
            }
        }
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
        if (maxUpgradeLevel == currentLevel) // Check if upgrade is max
            button.GetComponentInChildren<Text>().text = description + "\n - Max Level.";
        else
            button.GetComponentInChildren<Text>().text = description;
    }

    private void DisplayChoosenUpgrade(Upgrade upgrade)
    {
        int currentLevel = gameManager.upgradeLevelsDictionary[upgrade];
        UpgradesIdentifier upgradesIdentifier = upgradesModel.GetUpgradeEnum(upgrade, currentLevel + 1);
        if (upgradesIdentifier == UpgradesIdentifier.None)
            upgradesIdentifier = upgradesModel.GetUpgradeEnum(upgrade, currentLevel);

        upgradeDescriptionText.text = "Description:\n" + upgradesModel.GetRecord(upgradesIdentifier).Description;
    }

    public void Unpause()
    {
        Time.timeScale = 1.0f;
    }
    public void Pause()
    {
        Time.timeScale = 0.0f;
    }

    private void OnDisable()
    {
        foreach (var item in listOfUpgrades)
        {
            // Update Toggle enable
            Toggle toggle = item.Value.GetComponentInChildren<Toggle>();
            if (toggle)
            {
                gameManager.upgradeEnabled[item.Key] = toggle.isOn;
            }
            switch (item.Key)
            {
                case Upgrade.None:
                    break;
                case Upgrade.ExtraProjectiles:
                    Tower wife = GameObject.FindGameObjectWithTag("Wife")?.GetComponent<Tower>();
                    int wifeLevel = gameManager.upgradeLevelsDictionary[UpgradeMenu.Upgrade.ExtraProjectiles] - 1;
                    if (wifeLevel >= 0 && wife)
                        wife.isShooting = toggle.isOn;
                    break;
                case Upgrade.Ranged:
                    int rangedLevel = gameManager.upgradeLevelsDictionary[UpgradeMenu.Upgrade.Ranged] - 1;
                    Tower tower = ServiceLocator.Get<LevelManager>().towerInstance.GetComponent<Tower>();
                    if (rangedLevel >= 0)
                    {
                        if (toggle.isOn)
                            tower.range = tower.rangeAfterUpgrade;
                        else
                            tower.range = tower.rangeBeforeUpgrade;
                    }
                    break;
                case Upgrade.BarricadeReductionCost:
                    int barricadeLevel = gameManager.upgradeLevelsDictionary[UpgradeMenu.Upgrade.BarricadeReductionCost];
                    BarricadeSpawner barricadeSpawner = gameManager.barricadeSpawner;
                    if (barricadeLevel >= 1 && barricadeSpawner)
                    {
                        if (toggle.isOn)
                            barricadeSpawner.baseBarricadeCost = barricadeSpawner.costAfterUpgrade;
                        else
                            barricadeSpawner.baseBarricadeCost = barricadeSpawner.costBeforeUpgrade;
                    }
                    break;
                case Upgrade.TargetEnemy:
                    Dropdown dropdown = item.Value.GetComponentInChildren<Dropdown>();
                    string target = dropdown.options[dropdown.value].text;
                    if (!toggle.isOn)
                        target = "No Target";
                    gameManager.choosenTarget = target;
                    GameObject towerInstance = ServiceLocator.Get<LevelManager>().towerInstance;
                    towerInstance.GetComponent<Tower>().specificEnemy = gameManager.choosenTarget;
                    break;
                case Upgrade.FireProjectile:
                    int fireLevel = gameManager.upgradeLevelsDictionary[UpgradeMenu.Upgrade.FireProjectile] - 1;
                    Tower towerFire = ServiceLocator.Get<LevelManager>().towerInstance.GetComponent<Tower>();
                    if (fireLevel >= 0)
                    {
                        if (toggle.isOn)
                            towerFire.GetComponent<Tower>().damageType = DamageType.Fire;
                        else
                            towerFire.GetComponent<Tower>().damageType = DamageType.Normal;
                    }
                    break;
                case Upgrade.ImprovedBarricades:
                    // Updated in barricade script
                    break;
                case Upgrade.BarricadeSpawnRate:
                    int level = gameManager.upgradeLevelsDictionary[UpgradeMenu.Upgrade.BarricadeSpawnRate];
                    BarricadeSpawner barricadeSpawner_Rate = gameManager.barricadeSpawner;
                    if (level >= 1 && barricadeSpawner_Rate)
                    {
                        if (toggle.isOn)
                            barricadeSpawner_Rate.spawnCoolDownTime = barricadeSpawner_Rate.spawnCoolDownAfterUpgrade;
                        else
                            barricadeSpawner_Rate.spawnCoolDownTime = barricadeSpawner_Rate.spawnCoolDownBeforeUpgrade;
                    }
                    break;
                case Upgrade.TrashSpawnRate:
                    var trashCans = GameObject.FindObjectsOfType<ResourceSpawner>();
                    int trashLevel = ServiceLocator.Get<GameManager>().upgradeLevelsDictionary[UpgradeMenu.Upgrade.TrashSpawnRate];
                    if (trashLevel >= 1)
                    {
                        foreach (var go in trashCans)
                        {
                            if (toggle.isOn)
                                go.totalCoolTime = go.coolTimeAfterUpgrade;
                            else
                                go.totalCoolTime = go.coolTimeBeforeUpgrade;
                        }
                    }
                    break;
                case Upgrade.ImprovedPlayerHP:
                    // Cannot toggle
                    break;
                case Upgrade.ImprovedHealing:
                    int levelHealing = ServiceLocator.Get<GameManager>().upgradeLevelsDictionary[UpgradeMenu.Upgrade.ImprovedHealing];
                    Tower towerHealing = ServiceLocator.Get<LevelManager>().towerInstance.GetComponent<Tower>();
                    if (levelHealing >= 1)
                    {
                        if (toggle.isOn)
                            towerHealing.towerHealCostValue = towerHealing.towerHealAfterUpgrade;
                        else
                            towerHealing.towerHealCostValue = towerHealing.towerHealBeforeUpgrade;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}