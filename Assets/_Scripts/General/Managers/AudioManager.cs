using UnityEngine.Audio;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

//https://www.youtube.com/watch?v=6OT43pvUyfY
public class AudioManager : Singleton<AudioManager>
{
    public List<Sound> sounds = new();

    [SerializeField] AudioMixer _mixer;

    AudioSource _musicAudioSource;
    AudioSource _ambienceAudioSource;
    AudioSource _dialogueAudioSource;
    List<AudioSource> _sfxAudioSources = new();
    List<AudioSource> _uiAudioSources = new();

    IEnumerator _xFadeMusicCoroutine;
    IEnumerator _xFadeAmbienceCoroutine;

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

        if (_musicAudioSource.isPlaying)
        {
            _xFadeMusicCoroutine = CrossFadeCoroutine(_musicAudioSource, 5, 5, sound);
            StartCoroutine(_xFadeMusicCoroutine);
        }
        else
        {
            _xFadeMusicCoroutine = FadeInCoroutine(_musicAudioSource, sound, 5);
            StartCoroutine(_xFadeMusicCoroutine);
        }
    }

    public void PlayAmbience(Sound sound)
    {
        if (sound == null)
        {
            Debug.LogError("no ambience to play");
            return;
        }

        if (_ambienceAudioSource.isPlaying)
        {
            _xFadeAmbienceCoroutine = CrossFadeCoroutine(_ambienceAudioSource, 5, 5, sound);
            StartCoroutine(_xFadeAmbienceCoroutine);
        }
        else
        {
            _xFadeAmbienceCoroutine = FadeInCoroutine(_ambienceAudioSource, sound, 5);
            StartCoroutine(_xFadeAmbienceCoroutine);
        }
    }

    IEnumerator CrossFadeCoroutine(AudioSource audioSource, float fadeOutDuration, float fadeInDuration, Sound newSound)
    {
        Debug.Log($"Cross fade coroutine started.");
        yield return FadeOutCoroutine(audioSource, fadeOutDuration);
        yield return FadeInCoroutine(audioSource, newSound, fadeInDuration);
    }

    IEnumerator FadeOutCoroutine(AudioSource audioSource, float duration)
    {
        yield return audioSource.DOFade(0, duration)
                .SetUpdate(true)
                .WaitForCompletion();
    }

    IEnumerator FadeInCoroutine(AudioSource audioSource, Sound sound, float duration)
    {
        Debug.Log($"Fading {sound.name} in.");
        audioSource.pitch = sound.Pitch;
        audioSource.volume = 0;
        audioSource.clip = sound.Clips[0];
        audioSource.Play();

        yield return audioSource.DOFade(sound.Volume, duration)
                .SetUpdate(true)
                .WaitForCompletion();
    }

    public void PlayDialogue(Sound sound)
    {
        _dialogueAudioSource.pitch = sound.Pitch;
        _dialogueAudioSource.volume = sound.Volume;
        sound.Play(_dialogueAudioSource);
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
        Sound sound = sounds.First(s => s.name == soundName);
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
        Sound sound = sounds.First(s => s.name == soundName);
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
        Sound s = sounds.First(s => s.name == name);
        if (s == null)
            Debug.LogError($"No sound with name {name} in library");
        return s;
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

    public void SetUIVolume(float volume)
    {
        Debug.Log($"set ui volume {volume}");
        _mixer.SetFloat("UIVolume", Mathf.Log(volume) * 20);
    }

}
