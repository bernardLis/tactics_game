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

    [SerializeField] AudioMixer _mixer;

    AudioSource _musicAudioSource;
    AudioSource _ambienceAudioSource;
    AudioSource _dialogueAudioSource;

    List<AudioSource> _sfxAudioSources = new();

    protected override void Awake()
    {
        base.Awake();

        PopulateAudioSources();
        SetPlayerPrefVolume();
    }

    void PopulateAudioSources()
    {
        GameObject musicGameObject = new("Music");
        musicGameObject.transform.parent = transform;
        _musicAudioSource = musicGameObject.AddComponent<AudioSource>();
        _musicAudioSource.outputAudioMixerGroup = _mixer.FindMatchingGroups("Music")[0];

        GameObject ambienceGameObject = new("Ambience");
        ambienceGameObject.transform.parent = transform;
        _ambienceAudioSource = ambienceGameObject.AddComponent<AudioSource>();
        _ambienceAudioSource.loop = true;
        _ambienceAudioSource.outputAudioMixerGroup = _mixer.FindMatchingGroups("Ambience")[0];


        GameObject dialogueGameObject = new("Dialogue");
        dialogueGameObject.transform.parent = transform;
        _dialogueAudioSource = dialogueGameObject.AddComponent<AudioSource>();
        _dialogueAudioSource.outputAudioMixerGroup = _mixer.FindMatchingGroups("Dialogue")[0];

        for (int i = 0; i < 11; i++)
        {
            GameObject sfxGameObject = new("SFX" + i);
            sfxGameObject.transform.parent = transform;
            AudioSource a = sfxGameObject.AddComponent<AudioSource>();
            a.outputAudioMixerGroup = _mixer.FindMatchingGroups("SFX")[0];

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
    public void PlayDialogue(Sound sound)
    {
        _dialogueAudioSource.pitch = sound.Pitch;
        _dialogueAudioSource.volume = sound.Volume;
        sound.Play(_dialogueAudioSource);
    }

    public void StopDialogue()
    {
        _dialogueAudioSource.Stop();
    }

    public void PlaySFX(string soundName, Vector3 pos)
    {
        AudioSource a = _sfxAudioSources.FirstOrDefault(s => s.isPlaying == false);
        if (a == null)
            return;
        
        a.loop = false;
        a.gameObject.transform.position = pos; // it assumes that gameManager is at 0,0
        PlaySound(a, soundName);
    }

    public AudioSource PlaySFX(Sound sound, Vector3 pos, bool isLooping = false)
    {
        AudioSource a = _sfxAudioSources.FirstOrDefault(s => s.isPlaying == false);
        if (a == null)
            return null;
        a.pitch = sound.Pitch;
        a.volume = sound.Volume;
        a.gameObject.transform.position = pos; // it assumes that gameManager is at 0,0
        a.loop = isLooping;
        sound.Play(a);

        return a;
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

    /* volume setters */
    void SetPlayerPrefVolume()
    {
        SetMasterVolume(PlayerPrefs.GetFloat("Master", 1));
        SetMusicVolume(PlayerPrefs.GetFloat("Master", 1));
        SetAmbienceVolume(PlayerPrefs.GetFloat("Master", 1));
        SetDialogueVolume(PlayerPrefs.GetFloat("Master", 1));
        SetSFXVolume(PlayerPrefs.GetFloat("Master", 1));
    }

    // https://forum.unity.com/threads/changing-audio-mixer-group-volume-with-ui-slider.297884/ 
    public void SetMasterVolume(float volume)
    {
        _mixer.SetFloat("MasterVolume", Mathf.Log(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        _mixer.SetFloat("MusicVolume", Mathf.Log(volume) * 20);
    }

    public void SetAmbienceVolume(float volume)
    {
        _mixer.SetFloat("AmbienceVolume", Mathf.Log(volume) * 20);
    }

    public void SetDialogueVolume(float volume)
    {
        _mixer.SetFloat("DialogueVolume", Mathf.Log(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        _mixer.SetFloat("SFXVolume", Mathf.Log(volume) * 20);
    }
}
