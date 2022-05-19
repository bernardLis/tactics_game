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

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>(); // TODO: sound this does not work
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    public void PlaySound(string soundName)
    {
        Sound sound = sounds.First(s => s.name == soundName);
        if (sound == null)
        {
            Debug.LogError($"No sound {soundName} in library");
            return;
        }
        /*MissingComponentException: There is no 'AudioSource' attached to the "GameManager" game object, but a script is trying to access it.
        _audioSource.clip = sound.clip;
        _audioSource.Play();
        */
    }


}
