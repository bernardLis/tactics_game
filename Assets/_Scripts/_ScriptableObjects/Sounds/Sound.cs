using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "ScriptableObject/General/Sound")]
[System.Serializable]
public class Sound : BaseScriptableObject
{
    public AudioClip[] Clips;
    [Range(0f, 1f)]
    public float Volume = 1f;
    [Range(0.1f, 3f)]
    public float Pitch;

    public bool isPitchRandomized;
    public Vector2 PitchRange;

    [HideInInspector] public AudioSource Source;

    public void Play(AudioSource audioSource)
    {
        if (isPitchRandomized)
            audioSource.pitch = Random.Range(PitchRange.x, PitchRange.y);

        audioSource.clip = Clips[Random.Range(0, Clips.Length)];
        audioSource.Play();
    }
}
