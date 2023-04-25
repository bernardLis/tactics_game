using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/UI Effect Holder")]
public class EffectHolder : BaseScriptableObject
{
    
    [Header("VFX")]
    public GameObject VisualEffectPrefab;

    [Tooltip("-1 to play forever")]
    public float DurationSeconds;

    [Header("Sound")]
    public Sound Sound;

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
            AudioManager.Instance.PlaySFX(Sound, Vector3.zero);
        _effect = em.PlayEffect(VisualEffectPrefab, position, scale, DurationSeconds);
    }

    public void DestroyEffect()
    {
        if (!_effect)
            return;
        _effect.transform.DOScale(0, 0.3f).OnComplete(() => GameObject.Destroy(_effect));
    }
}
