using Lis.Core;
using UnityEngine;

namespace Lis.Units.Hero
{
    public class AnimationReceiver : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField]
        Sound _footstepSound;

        [SerializeField] Sound _landSound;
        AudioManager _audioManager;

        void Start()
        {
            _audioManager = AudioManager.Instance;
        }

        void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
                if (_footstepSound != null)
                    _audioManager.PlaySound(_footstepSound, transform.position);
        }

        // not used coz no jumps
        void OnLand(AnimationEvent animationEvent)
        {
            if (_landSound != null)
                _audioManager.PlaySound(_landSound, transform.position);
        }
    }
}