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
    Vector3 _scale;

    public async void PlayEffect(Vector3 position, Vector3 scale) { await PlayEffectAwaitable(position, scale); }

    public async Task PlayEffectAwaitable(Vector3 position, Vector3 scale)
    {
        InstantiateEffect(position, scale);

        if (Sound != null)
            AudioManager.Instance.PlaySFX(Sound, Vector3.zero);

        if (DurationSeconds == -1)
            return;
        await Task.Delay(Mathf.RoundToInt(DurationSeconds * 1000));
        DestroyEffect();
    }

    void InstantiateEffect(Vector3 pos, Vector3 scale)
    {
        _effect = GameObject.Instantiate(VisualEffectPrefab, pos, Quaternion.identity);
        _effect.transform.localScale = Vector3.zero;
        _effect.layer = Tags.UIVFXLayer;
        foreach (Transform child in _effect.transform)
            child.gameObject.layer = Tags.UIVFXLayer;

        _effect.transform.DOScale(scale, 0.3f);
    }

    public async void DestroyEffect()
    {
        await _effect.transform.DOScale(0, 0.3f).AsyncWaitForCompletion();
        GameObject.Destroy(_effect);
    }
}
