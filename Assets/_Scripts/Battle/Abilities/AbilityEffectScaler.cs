using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityEffectScaler : MonoBehaviour
{
    [SerializeField] List<Transform> _transformsToScale;
    [SerializeField] List<Light> _lightsToScale;

    public virtual void Initialize(float scale)
    {
        foreach (Transform t in _transformsToScale)
            t.localScale *= scale;
        foreach (Light l in _lightsToScale)
            l.range *= scale;

    }
}
