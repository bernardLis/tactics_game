using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Lis.Core
{
    [CreateAssetMenu(menuName = "ScriptableObject/Core/Sound")]
    [Serializable]
    public class Sound : BaseScriptableObject
    {
        public AudioClip[] Clips;

        [Range(0f, 1f)]
        public float Volume = 1f;

        [Range(0.1f, 3f)]
        public float Pitch = 1f;

        public bool IsPitchRandomized;
        public Vector2 PitchRange;
        public bool Loop;
        public bool PlayOnAwake = true;
        public int MaxDistance = 50;

        public bool IsFrequentSound;

        public AudioMixerGroup MixerGroup;
    }
}