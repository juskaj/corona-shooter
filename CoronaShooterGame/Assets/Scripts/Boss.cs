using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss
{
    public string Name;
    public Sprite Sprite;
    public float Cooldown;
    public int Health;
    public float ProjectileSpeed;

    public Boss(string name, Sprite sprite, float cooldown, int health, float speed)
    {
        Name = name;
        Sprite = sprite;
        Cooldown = cooldown;
        Health = health;
        ProjectileSpeed = speed;
    }
}
