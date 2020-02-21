using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStats : MonoBehaviour
{
    public class LevelStats<T>
    {
        public int level;
        public T value;
        public float cost;
    }

    public UpgradeStats Initialize()
    {
        return this;
    }

    [Header("More projectiles")]
    public List<float> throwingSpeed;
    public List<float> moreProjectileCost;
    public List<string> moreProjectileDescription;

    [Header("Long range")]
    public List<float> towerRange;
    public List<float> longRangedCost;
    public List<string> longRangedDescription;

    [Header("Barricades cost")]
    public List<float> barricadeCostReduction;
    public List<float> baricadeUpgradeCost;
    public List<string> barricadeDescription;

    [Header("Target Specific enemies")]
    public List<string> targetEnemy;
    public List<string> targetEnmeyUpgradeCost;
    public List<string> targetEnmeyDescription;

    [Header("Fire projectiles")]
    public List<float> fireDuration;
    public List<float> fireUpgradeCost;
    public List<string> fireDescription;
}
