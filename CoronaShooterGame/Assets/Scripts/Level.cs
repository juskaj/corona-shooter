using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Level
{
    public string Name;
    public List<Wave> Waves;
    public BossWave BossWave;
    public Sprite BackgroundSprite;
    public string LevelSoundtrack;
    public bool Playable;
    public int highScore;

    private int currentWave;

    public Level(string name, List<Wave> waves, BossWave bossWave, Sprite backgroundSprite, string soundTrackName, bool playable)
    {
        Name = name;
        Waves = waves;
        BossWave = bossWave;
        BackgroundSprite = backgroundSprite;
        LevelSoundtrack = soundTrackName;
        currentWave = 0;
        highScore = 0;
        Playable = playable;
    }

    public void ResetWaves()
    {
        currentWave = 0;
    }

    public bool IsBossWave()
    {
        return currentWave - 1 >= Waves.Count ? true : false;  
    }

    public Wave NextWave()
    {
        if (currentWave >= Waves.Count)
        {
            currentWave++;
            return BossWave;
        }
        return Waves[currentWave++];
    }
}
