

using UnityEngine;

namespace Lis
{
    public class BattleHeroAnimationReceiver : MonoBehaviour
    {
        AudioManager _audioManager;
        [Header("Audio")]
        [SerializeField] Sound _footstepSound;
        [SerializeField] Sound _landSound;


        void Start()
        {
            _audioManager = AudioManager.Instance;
        }

        void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
                if (_footstepSound != null)
                    _audioManager.PlaySFX(_footstepSound, transform.position);
        }

        // not used coz no jumps
        void OnLand(AnimationEvent animationEvent)
        {
            if (_landSound != null)
                _audioManager.PlaySFX(_landSound, transform.position);
        }
    }
}
