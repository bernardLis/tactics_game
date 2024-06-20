using System.Collections;
using System.Collections.Generic;
using Lis.Core;
using UnityEngine;

namespace Lis
{
    //https://www.youtube.com/watch?v=BgpqoRFCNOs&
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        public LinkedListNode<SoundEmitter> Node { get; set; }

        public Sound Sound { get; private set; }
        AudioManager _audioManager;
        AudioSource _audioSource;
        Coroutine _playCoroutine;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioManager = AudioManager.Instance;
        }

        public void Initialize(Sound sound)
        {
            Sound = sound;

            _audioSource.clip = sound.Clips[Random.Range(0, sound.Clips.Length)];
            _audioSource.volume = sound.Volume;
            _audioSource.pitch = sound.Pitch;
            if (sound.IsPitchRandomized) _audioSource.pitch = Random.Range(sound.PitchRange.x, sound.PitchRange.y);
            _audioSource.loop = sound.Loop;
            _audioSource.outputAudioMixerGroup = sound.MixerGroup;
            _audioSource.playOnAwake = sound.PlayOnAwake;
            _audioSource.maxDistance = sound.MaxDistance;
        }

        public void Play()
        {
            if (_playCoroutine != null)
                StopCoroutine(_playCoroutine);
            _audioSource.Play();
            _playCoroutine = StartCoroutine(WaitForSoundToEnd());
        }

        IEnumerator WaitForSoundToEnd()
        {
            yield return new WaitWhile(() => _audioSource.isPlaying);
            _audioManager.ReturnSoundEmitterToPool(this);
        }

        public void Stop()
        {
            if (_playCoroutine != null)
            {
                StopCoroutine(_playCoroutine);
                _playCoroutine = null;
            }

            _audioSource.Stop();
            _audioManager.ReturnSoundEmitterToPool(this);
        }

        public void Pause()
        {
            _audioSource.Pause();
        }

        public void Resume()
        {
            _audioSource.UnPause();
        }
    }
}