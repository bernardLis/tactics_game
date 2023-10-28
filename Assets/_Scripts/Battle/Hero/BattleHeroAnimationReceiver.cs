using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHeroAnimationReceiver : MonoBehaviour
{
    AudioManager _audioManager;
    [Header("Audio")]
    [SerializeField] Sound _footstepSound;

    void Start()
    {
        _audioManager = AudioManager.Instance;
    }

    void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
            if (_footstepSound != null)
                _audioManager.PlaySFX(_footstepSound, transform.TransformPoint(transform.position));
    }

    // not used coz no jumps
    void OnLand(AnimationEvent animationEvent)
    {
    }
}
