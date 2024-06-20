using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

//https://www.youtube.com/watch?v=6OT43pvUyfY
namespace Lis.Core
{
    public class AudioManager : Singleton<AudioManager>
    {
        public List<Sound> Sounds = new();

        [SerializeField] AudioMixer _mixer;
        [SerializeField] SoundEmitter _soundEmitterPrefab;

        int _currentMusicClipIndex;
        Sound _currentMusicSound;
        List<AudioSource> _musicAudioSources = new();
        AudioSource _mainMusicAudioSource;
        AudioMixerGroup _musicMixerGroup;
        IEnumerator _xFadeMusicCoroutine;

        IObjectPool<SoundEmitter> _soundEmitterPool;
        readonly List<SoundEmitter> _activeSoundEmitters = new();
        public readonly LinkedList<SoundEmitter> FrequentSoundEmitters = new();

        readonly bool _collectionCheck = true;
        readonly int _defaultCapacity = 10;
        readonly int _maxPoolSize = 100;
        readonly int _maxSoundInstances = 15;

        protected override void Awake()
        {
            base.Awake();
            CreateMusicAudioSource();
            InitializeSoundEmitterPool();
            SetPlayerPrefVolume();
        }


        /* OTHER SOUNDS */

        public SoundBuilder CreateSound()
        {
            return new(this);
        }

        public bool CanPlaySound(Sound s)
        {
            if (!s.IsFrequentSound) return true;

            if (FrequentSoundEmitters.Count >= _maxSoundInstances)
            {
                try
                {
                    FrequentSoundEmitters.First.Value.Stop();
                    return true;
                }
                catch
                {
                    Debug.Log("SoundEmitter is already released");
                }

                return false;
            }

            return true;
        }


        public SoundEmitter GetSoundEmitter()
        {
            return _soundEmitterPool.Get();
        }

        public void ReturnSoundEmitterToPool(SoundEmitter soundEmitter)
        {
            if (soundEmitter.gameObject.activeSelf)
                _soundEmitterPool.Release(soundEmitter);
        }


        SoundEmitter CreateSoundEmitter()
        {
            SoundEmitter soundEmitter = Instantiate(_soundEmitterPrefab, transform);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }

        void OnTakeFromPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(true);
            _activeSoundEmitters.Add(soundEmitter);
        }

        void OnReturnToPool(SoundEmitter soundEmitter)
        {
            if (soundEmitter.Node != null)
            {
                FrequentSoundEmitters.Remove(soundEmitter.Node);
                soundEmitter.Node = null;
            }

            soundEmitter.transform.parent = transform;
            soundEmitter.gameObject.SetActive(false);
            _activeSoundEmitters.Remove(soundEmitter);
        }

        void OnPoolDestroy(SoundEmitter soundEmitter)
        {
            Destroy(soundEmitter.gameObject);
        }

        void InitializeSoundEmitterPool()
        {
            _soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnToPool,
                OnPoolDestroy,
                _collectionCheck,
                _defaultCapacity,
                _maxPoolSize);
        }

        // TODO: not the right way to do it
        public void BattleSfxCleanup()
        {
            _soundEmitterPool.Clear();
        }

        public Sound GetSound(string n)
        {
            Sound s = Sounds.First(s => s.name == n);
            if (s == null)
                Debug.LogError($"No sound with name {n} in library");
            return s;
        }

        public void StopAllSounds()
        {
            foreach (SoundEmitter soundEmitter in _activeSoundEmitters)
                soundEmitter.Stop();
        }


        /* MUSIC */
        void CreateMusicAudioSource()
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject musicGameObject = new($"Music{i}");
                musicGameObject.transform.parent = transform;
                AudioSource mas = musicGameObject.AddComponent<AudioSource>();
                mas.loop = false;
                mas.spatialBlend = 0;
                mas.rolloffMode = AudioRolloffMode.Custom;
                mas.maxDistance = 99999;
                mas.outputAudioMixerGroup = _mixer.FindMatchingGroups("Music")[0];
                _musicAudioSources.Add(mas);
            }
        }

        public void PlayMusic(Sound sound)
        {
            if (sound == null)
            {
                Debug.LogError("No music to play");
                return;
            }

            Debug.Log($"Playing music {sound.name}");

            _currentMusicSound = sound;
            _currentMusicClipIndex = 0;
            StartCoroutine(PlayMusicCoroutine(_currentMusicSound.Clips[_currentMusicClipIndex]));
        }

        bool IsMusicPlaying()
        {
            return _musicAudioSources.Any(mas => mas.isPlaying);
        }

        AudioSource GetFreeMusicAudioSource()
        {
            return _musicAudioSources.FirstOrDefault(mas => !mas.isPlaying);
        }

        IEnumerator PlayMusicCoroutine(AudioClip clip)
        {
            if (this == null) yield break;

            AudioSource previous = _mainMusicAudioSource;
            previous.DOKill();
            if (previous != null)
                previous.DOFade(0, 5)
                    .SetUpdate(true)
                    .OnComplete(() => previous.Stop());

            _mainMusicAudioSource = GetFreeMusicAudioSource();
            _mainMusicAudioSource.clip = clip;
            _mainMusicAudioSource.volume = 0;
            _mainMusicAudioSource.Play();
            _mainMusicAudioSource.DOFade(1, 5)
                .SetUpdate(true);

            yield return new WaitForSecondsRealtime(clip.length - 10);

            _currentMusicClipIndex++;
            if (_currentMusicClipIndex >= _currentMusicSound.Clips.Length)
                _currentMusicClipIndex = 0;

            StartCoroutine(PlayMusicCoroutine(_currentMusicSound.Clips[_currentMusicClipIndex]));
        }


        /* VOLUME */
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