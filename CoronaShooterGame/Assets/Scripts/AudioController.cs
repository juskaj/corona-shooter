using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public Sound[] Sounds;

    private void Awake()
    {
        foreach (Sound sound in Sounds)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = sound.AudioClip;
            source.volume = sound.Volume;
            source.pitch = sound.Pitch;
            source.loop = sound.Loop;
            sound.Source = source;
        }
    }

    public void StopAllSounds()
    {
        foreach (Sound sound in Sounds)
        {
            sound.Source.Stop();
        }
    }

    public void PauseAllSounds()
    {
        foreach (Sound sound in Sounds)
        {
            sound.Source.Pause();
        }
    }

    public void UnpauseAllSounds()
    {
        foreach (Sound sound in Sounds)
        {
            sound.Source.UnPause();
        }
    }

    public void PlaySound(string name)
    {
        Sound sound = FindSound(name);

        if (sound == null)
        {
            Debug.LogWarning("Name for sound not found");
            return;
        }

        sound.Source.Play();
    }

    public void StopSound(string name)
    {
        Sound sound = FindSound(name);

        if (sound == null)
        {
            Debug.LogWarning("Name for sound not found");
            return;
        }

        sound.Source.Stop();
    }

    private Sound FindSound(string name)
    {
        foreach (Sound sound in Sounds)
        {
            if (sound.Name.Equals(name))
            {
                return sound;
            }
        }

        return null;
    }
}
