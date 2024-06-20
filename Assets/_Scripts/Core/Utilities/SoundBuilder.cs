using UnityEngine;

namespace Lis.Core.Utilities
{
    //https://www.youtube.com/watch?v=BgpqoRFCNOs&t=335s
    public class SoundBuilder
    {
        readonly AudioManager _audioManager;
        Sound _sound;
        Vector3 _pos = Vector3.zero;
        Transform _parent;

        public SoundBuilder(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        public SoundBuilder WithSound(Sound sound)
        {
            _sound = sound;
            return this;
        }

        public SoundBuilder WithPosition(Vector3 pos)
        {
            _pos = pos;
            return this;
        }

        public SoundBuilder WithParent(Transform parent)
        {
            _parent = parent;
            return this;
        }

        public SoundEmitter Play()
        {
            if (!_audioManager.CanPlaySound(_sound)) return null;

            SoundEmitter soundEmitter = _audioManager.GetSoundEmitter();
            soundEmitter.Initialize(_sound);
            soundEmitter.gameObject.transform.position = _pos;
            if (_parent != null) soundEmitter.transform.parent = _parent;
            if (_sound.IsFrequentSound) _audioManager.FrequentSoundEmittersQueue.Enqueue(soundEmitter);

            soundEmitter.Play();
            return soundEmitter;
        }
    }
}