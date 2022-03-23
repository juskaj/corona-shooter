using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWave : Wave
{
    public string BossName;
    public string BossSoundtrack;

    public BossWave(string waveName, Texture2D wavePicture, string bossName, string bossSoundrack) : base(waveName, wavePicture)
    {
        BossName = bossName;
        BossSoundtrack = bossSoundrack;
    }
}
