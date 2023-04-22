using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ThunderExecutor : AbilityExecutor
{
    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        Debug.Log($"executing thunder on {_entitiesInArea.Count}");

        foreach (BattleEntity entity in _entitiesInArea)
        {
            _damageDealt += Mathf.RoundToInt(entity.Stats.CalculateDamage(_selectedAbility));
            StartCoroutine(entity.GetHit(null, _selectedAbility));
        }
        CreateBattleLog();

        yield return new WaitForSeconds(3f);
        yield return _effectInstance.transform.DOScale(0f, 1f)
                .SetEase(Ease.OutCubic)
                .WaitForCompletion();

        CancelAbility();
    }
}

