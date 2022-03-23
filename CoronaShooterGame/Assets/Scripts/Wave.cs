using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave
{
    public string WaveName;
    public Texture2D WavePicture;

    public Wave(string waveName, Texture2D wavePicture)
    {
        WaveName = waveName;
        WavePicture = wavePicture;
    }
}
