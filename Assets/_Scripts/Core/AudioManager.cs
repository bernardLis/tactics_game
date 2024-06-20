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
        AudioSource _musicAudioSource;
        AudioMixerGroup _musicMixerGroup;
        IEnumerator _xFadeMusicCoroutine;

        IObjectPool<SoundEmitter> _soundEmitterPool;
        readonly List<SoundEmitter> _activeSfxSoundEmitters = new();

        public readonly Dictionary<Sound, int> Counts = new();

        readonly bool _collectionCheck = true;
        readonly int _defaultCapacity = 10;
        readonly int _maxPoolSize = 100;
        readonly int _maxSoundInstances = 30;

        protected override void Awake()
        {
            base.Awake();
            CreateMusicAudioSource();
            InitializeSoundEmitterPool();
            SetPlayerPrefVolume();
        }

        /* MUSIC */
        void CreateMusicAudioSource()
        {
            GameObject musicGameObject = new("Music");
            musicGameObject.transform.parent = transform;
            _musicAudioSource = musicGameObject.AddComponent<AudioSource>();
            _musicAudioSource.loop = true;
            _musicAudioSource.spatialBlend = 0;
            _musicAudioSource.rolloffMode = AudioRolloffMode.Custom;
            _musicAudioSource.maxDistance = 99999;
            _musicAudioSource.outputAudioMixerGroup = _mixer.FindMatchingGroups("Music")[0];
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
            _musicAudioSource.pitch = sound.Pitch;
            StartCoroutine(PlayMusicCoroutine());
        }

        IEnumerator PlayMusicCoroutine()
        {
            if (this == null) yield break;

            if (_musicAudioSource.isPlaying)
                yield return _musicAudioSource.DOFade(0, 5)
                    .SetUpdate(true)
                    .WaitForCompletion();

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

        /* OTHER SOUNDS */

        public SoundBuilder CreateSound()
        {
            return new SoundBuilder(this);
        }

        public SoundEmitter GetSoundEmitter()
        {
            return _soundEmitterPool.Get();
        }

        public void ReturnSoundEmitterToPool(SoundEmitter soundEmitter)
        {
            _soundEmitterPool.Release(soundEmitter);
        }

        public bool CanPlaySound(Sound s)
        {
            if (!Counts.TryGetValue(s, out var count))
                if (count >= _maxSoundInstances)
                    return false;
            return true;
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
            _activeSfxSoundEmitters.Add(soundEmitter);
        }

        void OnReturnToPool(SoundEmitter soundEmitter)
        {
            if (Counts.TryGetValue(soundEmitter.Sound, out var count))
                Counts[soundEmitter.Sound] = count - 1;

            soundEmitter.gameObject.SetActive(false);
            _activeSfxSoundEmitters.Remove(soundEmitter);
        }

        void OnPoolDestroy(SoundEmitter soundEmitter)
        {
            Destroy(soundEmitter.gameObject);
        }


        public void PlaySound(Sound sound)
        {
            PlaySound(sound, Vector3.zero);
        }

        public SoundEmitter PlaySound(string soundName)
        {
            Sound sound = GetSound(soundName);
            return PlaySound(sound, Vector3.zero);
        }

        public void PlaySound(string soundName, Vector3 pos)
        {
            Sound sound = GetSound(soundName);
            PlaySound(sound, pos);
        }

        public SoundEmitter PlaySound(Sound sound, Transform t)
        {
            SoundEmitter a = PlaySound(sound, t.position);
            a.transform.parent = t;
            return a;
        }

        public SoundEmitter PlaySound(Sound sound, Vector3 pos)
        {
            SoundEmitter a = GetSoundEmitter();
            a.gameObject.transform.position = pos;
            a.Initialize(sound);
            a.Play();
            return a;
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

        Sound GetSound(string n)
        {
            Sound s = Sounds.First(s => s.name == n);
            if (s == null)
                Debug.LogError($"No sound with name {n} in library");
            return s;
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