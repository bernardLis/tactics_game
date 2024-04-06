using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.Audio;

//https://www.youtube.com/watch?v=6OT43pvUyfY
namespace Lis.Core
{
    public class AudioManager : Singleton<AudioManager>
    {
        public List<Sound> Sounds = new();

        [SerializeField] AudioMixer _mixer;
        AudioMixerGroup _musicMixerGroup;
        AudioMixerGroup _ambienceMixerGroup;
        AudioMixerGroup _dialogueMixerGroup;
        AudioMixerGroup _sfxMixerGroup;
        AudioMixerGroup _uiMixerGroup;

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
            GetMixerGroups();
            PopulateAudioSources();
            SetPlayerPrefVolume();
        }

        void GetMixerGroups()
        {
            _musicMixerGroup = _mixer.FindMatchingGroups("Music")[0];
            _ambienceMixerGroup = _mixer.FindMatchingGroups("Ambience")[0];
            _dialogueMixerGroup = _mixer.FindMatchingGroups("Dialogue")[0];
            _sfxMixerGroup = _mixer.FindMatchingGroups("SFX")[0];
            _uiMixerGroup = _mixer.FindMatchingGroups("UI")[0];
        }

        void PopulateAudioSources()
        {
            GameObject musicGameObject = new("Music");
            musicGameObject.transform.parent = transform;
            _musicAudioSource = musicGameObject.AddComponent<AudioSource>();
            _musicAudioSource.loop = true;
            _musicAudioSource.outputAudioMixerGroup = _musicMixerGroup;

            GameObject ambienceGameObject = new("Ambience");
            ambienceGameObject.transform.parent = transform;
            _ambienceAudioSource = ambienceGameObject.AddComponent<AudioSource>();
            _ambienceAudioSource.loop = true;
            _ambienceAudioSource.outputAudioMixerGroup = _ambienceMixerGroup;

            GameObject dialogueGameObject = new("Dialogue");
            dialogueGameObject.transform.parent = transform;
            _dialogueAudioSource = dialogueGameObject.AddComponent<AudioSource>();
            _dialogueAudioSource.outputAudioMixerGroup = _dialogueMixerGroup;

            _sfxAudioSources = new();
            for (int i = 0; i < 25; i++)
                CreateSfxAudioSource();

            _uiAudioSources = new();
            for (int i = 0; i < 10; i++)
                CreateUiAudioSource();
        }

        AudioSource CreateSfxAudioSource()
        {
            GameObject sfxGameObject = new("SFX" + _sfxAudioSources.Count);
            sfxGameObject.transform.parent = transform;
            AudioSource a = sfxGameObject.AddComponent<AudioSource>();
            a.spatialBlend = 1;
            a.rolloffMode = AudioRolloffMode.Custom;
            a.maxDistance = 50;
            a.outputAudioMixerGroup = _sfxMixerGroup;

            _sfxAudioSources.Add(a);

            return a;
        }

        AudioSource CreateUiAudioSource()
        {
            GameObject uiGameObject = new("UI" + _uiAudioSources.Count);
            uiGameObject.transform.parent = transform;
            AudioSource a = uiGameObject.AddComponent<AudioSource>();
            a.outputAudioMixerGroup = _uiMixerGroup;

            _uiAudioSources.Add(a);

            return a;
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
            if (sound != null) return;
            Debug.LogError("no ambience to play");
        }

        public AudioSource PlayDialogue(Sound sound)
        {
            _dialogueAudioSource.pitch = sound.Pitch;
            _dialogueAudioSource.volume = sound.Volume;
            sound.Play(_dialogueAudioSource);

            return _dialogueAudioSource;
        }

        public void StopDialogue()
        {
            _dialogueAudioSource.Stop();
        }

        public void PlaySfxDelayed(string soundName, Vector3 pos, float delay)
        {
            StartCoroutine(PlaySfxDelayedCoroutine(soundName, pos, delay));
        }

        IEnumerator PlaySfxDelayedCoroutine(string soundName, Vector3 pos, float delay)
        {
            yield return new WaitForSeconds(delay);
            PlaySfx(soundName, pos);
        }

        public AudioSource PlaySfx(string soundName, Vector3 pos)
        {
            Sound sound = Sounds.First(s => s.name == soundName);
            if (sound == null)
            {
                Debug.LogError($"No sound {soundName} in library");
                return null;
            }

            return PlaySfx(sound, pos);
        }

        public AudioSource PlaySfx(Sound sound, Transform t, bool isLooping = false)
        {
            AudioSource a = PlaySfx(sound, t.position, isLooping);
            a.transform.parent = t;
            return a;
        }

        public AudioSource PlaySfx(Sound sound, Vector3 pos, bool isLooping = false)
        {
            AudioSource a = _sfxAudioSources.FirstOrDefault(s => s.isPlaying == false);
            if (a == null) a = CreateSfxAudioSource();
            if (a == null) return null;

            a.gameObject.transform.position = pos; // it assumes that gameManager is at 0,0
            a.loop = isLooping;
            Sound instance = Instantiate(sound);
            instance.Play(a);

            return a;
        }

        // TODO: not the right way to do it
        public void BattleSfxCleanup()
        {
            foreach (AudioSource a in _sfxAudioSources)
            {
                if (a.transform.parent != transform)
                    a.transform.parent = transform;
                if (a.isPlaying)
                    a.Stop();
            }
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
            if (a == null) a = CreateUiAudioSource();

            Sound instance = Instantiate(sound);
            instance.Play(a);

            return a;
        }

        public Sound GetSound(string n)
        {
            Sound s = Sounds.First(s => s.name == n);
            if (s == null)
                Debug.LogError($"No sound with name {n} in library");
            return s;
        }

        /* volume setters */
        void SetPlayerPrefVolume()
        {
            SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1));
            SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1));
            SetAmbienceVolume(PlayerPrefs.GetFloat("AmbienceVolume", 1));
            SetDialogueVolume(PlayerPrefs.GetFloat("DialogueVolume", 1));
            SetSfxVolume(PlayerPrefs.GetFloat("SFXVolume", 1));
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

        public void SetSfxVolume(float volume)
        {
            _mixer.SetFloat("SFXVolume", Mathf.Log(volume) * 20);
        }

        public void SetUIVolume(float volume)
        {
            _mixer.SetFloat("UIVolume", Mathf.Log(volume) * 20);
        }


        public void MuteAllButMusic()
        {
            _mixer.SetFloat("AmbienceVolume", -80);
            _mixer.SetFloat("DialogueVolume", -80);
            _mixer.SetFloat("SFXVolume", -80);
            _mixer.SetFloat("UIVolume", -80);
        }

        public void UnmuteAll()
        {
            SetPlayerPrefVolume();
        }
    }
}