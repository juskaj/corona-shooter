using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossType
{
    shooting,
    spawning
}

[System.Serializable]
public class Boss
{
    public string Name;
    public Sprite sprite;
    public BossType Type;
}
