using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FireballExecutor : AbilityExecutor
{
    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        Debug.Log($"Executing fireball on {_entitiesInArea.Count}");
        foreach (BattleEntity entity in _entitiesInArea)
        {
            _damageDealt += Mathf.RoundToInt(entity.Entity.CalculateDamage(_selectedAbility));
            StartCoroutine(entity.GetHit(_selectedAbility));
        }

        yield return new WaitForSeconds(6f);
        Transform[] allChildren = _effectInstance.GetComponentsInChildren<Transform>();

        foreach (Transform child in allChildren)
            child.DOScale(0f, 1f);

        yield return _effectInstance.transform.DOScale(0f, 1f).WaitForCompletion();
        CancelAbility();
    }

}
