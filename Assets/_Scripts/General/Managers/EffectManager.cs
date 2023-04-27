using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] List<EffectHolder> _effects = new();

    public GameObject PlayEffectWithName(string name, Vector3 position, Vector3 scale)
    {
        foreach (EffectHolder effect in _effects)
        {
            if (effect.name == name)
                return PlayEffect(effect.VisualEffectPrefab, position, scale, effect.DurationSeconds);
        }
        Debug.LogError($"No effect {name} found.");
        return null;
    }

    public GameObject PlayEffect(GameObject prefab, Vector3 position, Vector3 scale,
            float duration)
    {
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        instance.transform.localScale = scale;
        instance.layer = Tags.UIVFXLayer;
        foreach (Transform child in instance.transform)
            child.gameObject.layer = Tags.UIVFXLayer;

        if (duration == -1)
            return instance;

        StartCoroutine(EffectCoroutine(instance, duration));
        return instance;
    }

    IEnumerator EffectCoroutine(GameObject effectInstance, float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(effectInstance);
    }

}
