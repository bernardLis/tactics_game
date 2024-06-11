using Lis.Core;
using UnityEngine;

namespace Lis.Units.Hero
{
    public class AnimationReceiver : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField]
        private Sound _footstepSound;

        [SerializeField] private Sound _landSound;
        private AudioManager _audioManager;

        private void Start()
        {
            _audioManager = AudioManager.Instance;
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
                if (_footstepSound != null)
                    _audioManager.PlaySfx(_footstepSound, transform.position);
        }

        // not used coz no jumps
        private void OnLand(AnimationEvent animationEvent)
        {
            if (_landSound != null)
                _audioManager.PlaySfx(_landSound, transform.position);
        }
    }
}