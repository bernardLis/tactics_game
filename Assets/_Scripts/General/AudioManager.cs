using UnityEngine.Audio;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

//https://www.youtube.com/watch?v=6OT43pvUyfY
public class AudioManager : Singleton<AudioManager>
{
    public List<Sound> sounds = new();

    AudioSource _musicAudioSource;
    AudioSource _ambienceAudioSource;
    AudioSource _dialogueAudioSource;

    List<AudioSource> _sfxAudioSources = new();

    protected override void Awake()
    {
        base.Awake();

        PopulateAudioSources();
    }

    void PopulateAudioSources()
    {
        GameObject musicGameObject = new("Music");
        musicGameObject.transform.parent = transform;
        _musicAudioSource = musicGameObject.AddComponent<AudioSource>();

        GameObject ambienceGameObject = new("Ambience");
        ambienceGameObject.transform.parent = transform;
        _ambienceAudioSource = ambienceGameObject.AddComponent<AudioSource>();
        _ambienceAudioSource.loop = true;

        GameObject dialogueGameObject = new("Dialogue");
        dialogueGameObject.transform.parent = transform;
        _dialogueAudioSource = dialogueGameObject.AddComponent<AudioSource>();

        for (int i = 0; i < 11; i++)
        {
            GameObject sfxGameObject = new("SFX" + i);
            sfxGameObject.transform.parent = transform;
            AudioSource a = sfxGameObject.AddComponent<AudioSource>();
            _sfxAudioSources.Add(a);
        }
    }

    public async void PlayMusic(Sound sound)
    {
        if (sound == null)
        {
            Debug.LogError("no music to play");
            return;
        }

        if (_musicAudioSource.isPlaying)
        {
            await FadeOut(_musicAudioSource, 5);
            await FadeIn(_musicAudioSource, sound, 10);
        }
        else
        {
            await FadeIn(_musicAudioSource, sound, 10);
        }
    }

    public async void PlayAmbience(Sound sound)
    {
        if (sound == null)
        {
            Debug.LogError("no ambience to play");
            return;
        }

        if (_ambienceAudioSource.isPlaying)
        {
            await FadeOut(_ambienceAudioSource, 5);
            await FadeIn(_ambienceAudioSource, sound, 10);
        }
        else
        {
            await FadeIn(_ambienceAudioSource, sound, 10);
        }
    }

    public void PlayDialogue(string soundName)
    {
        PlaySound(_dialogueAudioSource, soundName);
    }

    public void PlaySFX(string soundName, Vector3 pos)
    {
        AudioSource a = _sfxAudioSources.FirstOrDefault(s => s.isPlaying == false);
        a.gameObject.transform.position = pos; // it assumes that gameManager is at 0,0
        PlaySound(a, soundName);
    }

    public void PlaySFX(Sound sound, Vector3 pos)
    {
        AudioSource a = _sfxAudioSources.FirstOrDefault(s => s.isPlaying == false);
        a.pitch = sound.Pitch;
        a.volume = sound.Volume;
        a.gameObject.transform.position = pos; // it assumes that gameManager is at 0,0
        sound.Play(a);
    }

    public void PlaySound(AudioSource audioSource, string soundName)
    {
        Sound sound = sounds.First(s => s.name == soundName);
        audioSource.pitch = sound.Pitch;
        audioSource.volume = sound.Volume;
        if (sound == null)
        {
            Debug.LogError($"No sound {soundName} in library");
            return;
        }
        sound.Play(audioSource);
    }

    async Task FadeOut(AudioSource audioSource, int duration)
    {
        float currentTime = 0f;
        float start = audioSource.volume;
        float end = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, end, currentTime / duration);
            await Task.Yield();
        }
    }

    async Task FadeIn(AudioSource audioSource, Sound sound, int duration)
    {
        audioSource.pitch = sound.Pitch;
        audioSource.volume = 0;
        audioSource.clip = sound.Clips[0];
        audioSource.Play();

        float currentTime = 0f;
        float start = 0f;
        float end = sound.Volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, end, currentTime / duration);
            await Task.Yield();
        }
    }

}
