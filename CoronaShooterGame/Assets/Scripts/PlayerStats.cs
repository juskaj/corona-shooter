using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    public int Score { get; set; }
    public int DamageLevel { get; set; }
    public int CooldownLevel { get; set; }
    public int HealthLevel { get; set; }
    public int SpeedLevel { get; set; }

    public PlayerStats(int score, int damageLevel, int cooldownLevel, int healthLevel, int speedLevel)
    {
        Score = score;
        DamageLevel = damageLevel;
        CooldownLevel = cooldownLevel;
        HealthLevel = healthLevel;
        SpeedLevel = speedLevel;
    }

    public int UpgradeDamage()
    {
        int upgradeCost = (int)Mathf.Pow(DamageLevel, 2f) * 100;

        if (Score >= upgradeCost)
        {
            DamageLevel++;
            Score -= upgradeCost;
        }
        return DamageLevel;
    }

    public int UpgradeHealth()
    {
        int upgradeCost = (int)Mathf.Pow(HealthLevel, 2f) * 100;

        if (Score >= upgradeCost)
        {
            HealthLevel++;
            Score -= upgradeCost;
        }
        return HealthLevel;
    }

    public int UpgradeSpeed()
    {
        int upgradeCost = (int)Mathf.Pow(SpeedLevel, 2f) * 100;

        if (Score >= upgradeCost)
        {
            SpeedLevel++;
            Score -= upgradeCost;
        }
        return SpeedLevel;
    }

    public int UpgradeCooldown()
    {
        int upgradeCost = (int)Mathf.Pow(CooldownLevel, 2f) * 100;

        if (Score >= upgradeCost)
        {
            CooldownLevel++;
            Score -= upgradeCost;
        }
        return CooldownLevel;
    }
}