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

    private int highScore;
    private int currentWave;

    public Level(string name, List<Wave> waves, BossWave bossWave, Sprite backgroundSprite, string soundTrackName)
    {
        Name = name;
        Waves = waves;
        BossWave = bossWave;
        BackgroundSprite = backgroundSprite;
        LevelSoundtrack = soundTrackName;
        currentWave = 0;
        highScore = 0;
    }

    public void ResetWaves()
    {
        currentWave = 0;
    }

    public Wave NextWave()
    {
        Debug.Log(currentWave);
        Debug.Log(Waves.Count);
        if (currentWave >= Waves.Count)
        {
            return BossWave;
        }
        return Waves[currentWave++];
    }
}
