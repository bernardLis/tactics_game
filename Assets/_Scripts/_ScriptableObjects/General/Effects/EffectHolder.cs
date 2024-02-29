

using DG.Tweening;
using Lis.Core;
using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/UI Effect Holder")]
    public class EffectHolder : BaseScriptableObject
    {

        [Header("VFX")]
        public GameObject VisualEffectPrefab;

        [Tooltip("-1 to play forever")]
        public float DurationSeconds;

        [Header("Sound")]
        public Sound Sound;

        [HideInInspector] public AudioSource SFXAudioSource;

        GameObject _effect;

        public void PlayEffect(Vector3 position, Vector3 scale)
        {
            EffectManager em = GameManager.Instance.GetComponent<EffectManager>();
            if (em == null)
            {
                Debug.LogWarning($"No effect manager, can't play effect {name}.");
                return;
            }

            if (Sound != null)
                SFXAudioSource = AudioManager.Instance.PlaySFX(Sound, position);
            _effect = em.PlayEffect(VisualEffectPrefab, position, scale, DurationSeconds);
        }

        public void DestroyEffect()
        {
            if (!_effect)
                return;
            _effect.transform.DOScale(0, 0.3f).OnComplete(() => GameObject.Destroy(_effect));
        }
    }
}
