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
}
