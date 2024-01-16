using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAbilityObject : MonoBehaviour
{
    protected Ability _ability;
    public virtual void Initialize(Ability ability)
    {
        _ability = ability;
        _ability.OnLevelUp += OnAbilityLevelUp;

    }

    protected virtual void OnAbilityLevelUp()
    {
        // override
    }

    void OnDestroy() => _ability.OnLevelUp -= OnAbilityLevelUp;

    public virtual void Execute(Vector3 pos, Quaternion rot)
    {
        transform.localPosition = pos;
        transform.localRotation = rot;
        gameObject.SetActive(true);
        StartCoroutine(ExecuteCoroutine());
    }

    protected virtual IEnumerator ExecuteCoroutine()
    {
        yield return null;
    }
}
