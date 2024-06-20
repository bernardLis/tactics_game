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

        public void Play()
        {
            if (!_audioManager.CanPlaySound(_sound)) return;

            SoundEmitter soundEmitter = _audioManager.GetSoundEmitter();
            soundEmitter.Initialize(_sound);
            soundEmitter.gameObject.transform.position = _pos;
            if (_parent != null) soundEmitter.transform.parent = _parent;

            if (_audioManager.Counts.TryGetValue(_sound, out var count))
                _audioManager.Counts[_sound] = count + 1;
            else
                _audioManager.Counts.Add(_sound, 1);

            soundEmitter.Play();
        }
    }
}