using UnityEngine.Audio;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

//https://www.youtube.com/watch?v=6OT43pvUyfY
public class AudioManager : Singleton<AudioManager>
{
    public List<Sound> Sounds = new();

    [SerializeField] AudioMixer _mixer;

    AudioSource _musicAudioSource;
    AudioSource _ambienceAudioSource;
    AudioSource _dialogueAudioSource;
    List<AudioSource> _sfxAudioSources = new();
    List<AudioSource> _uiAudioSources = new();

    IEnumerator _xFadeMusicCoroutine;
    IEnumerator _xFadeAmbienceCoroutine;

    Sound _currentMusicSound;
    int _currentMusicClipIndex;

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
        _musicAudioSource.loop = true;
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

        _sfxAudioSources = new();
        for (int i = 0; i < 25; i++)
        {
            GameObject sfxGameObject = new("SFX" + i);
            sfxGameObject.transform.parent = transform;
            AudioSource a = sfxGameObject.AddComponent<AudioSource>();
            a.spatialBlend = 1;
            a.rolloffMode = AudioRolloffMode.Custom;
            a.maxDistance = 50;
            a.outputAudioMixerGroup = _mixer.FindMatchingGroups("SFX")[0];

            _sfxAudioSources.Add(a);
        }

        _uiAudioSources = new();
        for (int i = 0; i < 10; i++)
        {
            GameObject uiGameObject = new("UI" + i);
            uiGameObject.transform.parent = transform;
            AudioSource a = uiGameObject.AddComponent<AudioSource>();
            a.outputAudioMixerGroup = _mixer.FindMatchingGroups("UI")[0];

            _uiAudioSources.Add(a);
        }

    }

    public void PlayMusic(Sound sound)
    {
        if (sound == null)
        {
            Debug.LogError("No music to play");
            return;
        }
        _currentMusicSound = sound;
        _currentMusicClipIndex = 0;
        _musicAudioSource.pitch = sound.Pitch;
        StartCoroutine(PlayMusicCoroutine());
    }

    IEnumerator PlayMusicCoroutine()
    {
        if (_musicAudioSource.isPlaying)
        {
            yield return _musicAudioSource.DOFade(0, 5)
                    .SetUpdate(true)
                    .WaitForCompletion();
        }

        _musicAudioSource.volume = 0;
        _musicAudioSource.clip = _currentMusicSound.Clips[_currentMusicClipIndex];
        _musicAudioSource.Play();

        yield return _musicAudioSource.DOFade(_currentMusicSound.Volume, 5)
                .SetUpdate(true)
                .WaitForCompletion();

        yield return new WaitForSecondsRealtime(_musicAudioSource.clip.length - 10);

        _currentMusicClipIndex++;
        if (_currentMusicClipIndex >= _currentMusicSound.Clips.Length)
            _currentMusicClipIndex = 0;

        StartCoroutine(PlayMusicCoroutine());
    }

    // TODO: I never use ambience, but it should be handled similarly to music, 
    // I just don't know how to use one coroutine schema for both...
    public void PlayAmbience(Sound sound)
    {
        Debug.LogError($"not implemented");
        if (sound == null)
        {
            Debug.LogError("no ambience to play");
            return;
        }
    }

    public AudioSource PlayDialogue(Sound sound)
    {
        _dialogueAudioSource.pitch = sound.Pitch;
        _dialogueAudioSource.volume = sound.Volume;
        sound.Play(_dialogueAudioSource);

        return _dialogueAudioSource;
    }
    public void StopDialogue() { _dialogueAudioSource.Stop(); }

    public void PlaySFXDelayed(string soundName, Vector3 pos, float delay)
    {
        StartCoroutine(PlaySFXDelayedCoroutine(soundName, pos, delay));
    }
    IEnumerator PlaySFXDelayedCoroutine(string soundName, Vector3 pos, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaySFX(soundName, pos);
    }

    public AudioSource PlaySFX(string soundName, Vector3 pos)
    {
        Sound sound = Sounds.First(s => s.name == soundName);
        if (sound == null)
        {
            Debug.LogError($"No sound {soundName} in library");
            return null;
        }

        return PlaySFX(sound, pos);
    }

    public AudioSource PlaySFX(Sound sound, Vector3 pos, bool isLooping = false)
    {
        AudioSource a = _sfxAudioSources.FirstOrDefault(s => s.isPlaying == false);

        if (a == null) return null;

        a.gameObject.transform.position = pos; // it assumes that gameManager is at 0,0
        a.loop = isLooping;
        Sound instance = Instantiate(sound);
        instance.Play(a);

        return a;
    }

    public void PlayUIDelayed(string soundName, float delay)
    {
        StartCoroutine(PlayUIDelayedCoroutine(soundName, delay));
    }
    IEnumerator PlayUIDelayedCoroutine(string soundName, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        PlayUI(soundName);
    }

    public AudioSource PlayUI(string soundName)
    {
        Sound sound = Sounds.First(s => s.name == soundName);
        if (sound == null)
        {
            Debug.LogError($"No sound {soundName} in library");
            return null;
        }

        return PlayUI(sound);
    }

    public AudioSource PlayUI(Sound sound)
    {
        AudioSource a = _uiAudioSources.FirstOrDefault(s => s.isPlaying == false);
        if (a == null) return null;

        Sound instance = Instantiate(sound);
        instance.Play(a);

        return a;
    }

    public Sound GetSound(string name)
    {
        Sound s = Sounds.First(s => s.name == name);
        if (s == null)
            Debug.LogError($"No sound with name {name} in library");
        return s;
    }

    /* volume setters */
    void SetPlayerPrefVolume()
    {
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1));
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1));
        SetAmbienceVolume(PlayerPrefs.GetFloat("AmbienceVolume", 1));
        SetDialogueVolume(PlayerPrefs.GetFloat("DialogueVolume", 1));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1));
        SetUIVolume(PlayerPrefs.GetFloat("UIVolume", 1));
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

    public void SetUIVolume(float volume)
    {
        _mixer.SetFloat("UIVolume", Mathf.Log(volume) * 20);
    }

}
