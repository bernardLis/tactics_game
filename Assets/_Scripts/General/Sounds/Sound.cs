using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "ScriptableObject/General/Sound")]
[System.Serializable]
public class Sound : BaseScriptableObject
{
    public AudioClip Clip;
    [Range(0f, 1f)]
    public float Volume;
    [Range(0.1f, 3f)]
    public float Pitch;

    [HideInInspector] public AudioSource Source;

    public void Play()
    {
        
        Debug.Log($"in play clip: {Clip}");
        if (Source == null)
        {
            AudioManager.Instance.CreateAudioSource(this);
            Debug.Log($"source after creation: {Source}");            
        }
        Source.Play();
    }
}
