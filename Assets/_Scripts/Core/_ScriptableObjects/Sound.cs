using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Core
{
    [CreateAssetMenu(menuName = "ScriptableObject/Core/Sound")]
    [System.Serializable]
    public class Sound : BaseScriptableObject
    {
        public AudioClip[] Clips;
        [Range(0f, 1f)]
        public float Volume = 1f;
        [Range(0.1f, 3f)]
        public float Pitch = 1f;

        [FormerlySerializedAs("isPitchRandomized")] public bool IsPitchRandomized;
        public Vector2 PitchRange;

        [HideInInspector] public AudioSource Source;

        public void Play(AudioSource audioSource)
        {
            audioSource.volume = Volume;
            audioSource.pitch = Pitch;
            if (IsPitchRandomized)
                audioSource.pitch = Random.Range(PitchRange.x, PitchRange.y);

            audioSource.clip = Clips[Random.Range(0, Clips.Length)];
            audioSource.Play();
        }
    }
}
