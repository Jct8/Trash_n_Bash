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

    [Header("More Projectiles")]
    public List<float> throwingSpeed;
    public List<float> moreProjectileCost;
    public List<string> moreProjectileDescription;

    [Header("Long Range")]
    public List<float> towerRange;
    public List<float> longRangedCost;
    public List<string> longRangedDescription;

    [Header("Barricades Cost")]
    public List<float> barricadeCostReduction;
    public List<float> baricadeUpgradeCost;
    public List<string> barricadeDescription;

    [Header("Target Specific Enemies")]
    public List<string> targetEnemy;
    public List<float> targetEnmeyUpgradeCost;
    public List<string> targetEnmeyDescription;

    [Header("Fire Projectiles")]
    public List<float> fireDuration;
    public List<float> fireUpgradeCost;
    public List<string> fireDescription;
}
