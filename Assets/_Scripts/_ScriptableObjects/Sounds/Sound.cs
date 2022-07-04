using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "ScriptableObject/General/Sound")]
[System.Serializable]
public class Sound : BaseScriptableObject
{
    public AudioClip[] Clips;
    [Range(0f, 1f)]
    public float Volume;
    [Range(0.1f, 3f)]
    public float Pitch;

    [HideInInspector] public AudioSource Source;

    public void Play(AudioSource audioSource)
    {
        audioSource.clip = Clips[Random.Range(0, Clips.Length)];
        audioSource.Play();
    }
}
