using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FireballExecutor : AbilityExecutor
{
    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        Debug.Log($"executing fireball on {_entitiesInArea.Count}");
        foreach (BattleEntity entity in _entitiesInArea)
            StartCoroutine(entity.GetHit(null, _selectedAbility));

        yield return new WaitForSeconds(3f);
        yield return _effectInstance.transform.DOScale(0f, 1f).WaitForCompletion();

        CancelAbility();
    }

}
