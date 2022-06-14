using UnityEngine.Audio;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//https://www.youtube.com/watch?v=6OT43pvUyfY
public class AudioManager : Singleton<AudioManager>
{
    public List<Sound> sounds = new();
    AudioSource _audioSource;

    protected override void Awake()
    {
        base.Awake();

        _audioSource = GetComponent<AudioSource>();

        foreach (Sound sound in sounds)
            CreateAudioSource(sound);
    }

    public void PlaySound(string soundName)
    {
        Sound sound = sounds.First(s => s.name == soundName);
        if (sound == null)
        {
            Debug.LogError($"No sound {soundName} in library");
            return;
        }
        sound.Play();
        /*MissingComponentException: There is no 'AudioSource' attached to the "GameManager" game object, but a script is trying to access it.
        _audioSource.clip = sound.clip;
        _audioSource.Play();
        */
    }

    public void PlaySound(Sound sound)
    {
        if (sounds.Contains(sound))
        {
            sound.Play();
            return;
        }

        sounds.Add(sound);
        CreateAudioSource(sound);
        sound.Play();
    }

    public void CreateAudioSource(Sound sound)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        sound.Source = source; // TODO: sound this does not work
        Debug.Log($"sound.clip: {sound.Clips[0]}");
        source.clip = sound.Clips[0];
        source.volume = sound.Volume;
        source.pitch = sound.Pitch;
    }

    public void RequestSource(Sound sound)
    {
        // it should be different
        // return gameObject.AddComponent<AudioSource>(); // TODO: sound this does not work
    }
}
