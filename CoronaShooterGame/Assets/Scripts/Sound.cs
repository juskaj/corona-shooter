using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string Name;

    public AudioClip AudioClip;

    [Range(0, 1)]
    public float Volume;
    [Range(0, 3)]
    public float Pitch;

    public bool Loop;

    [HideInInspector]
    public AudioSource Source;
}
