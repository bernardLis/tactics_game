using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ThunderExecutor : AbilityExecutor
{
    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        Debug.Log($"Executing thunder on {_entitiesInArea.Count}");

        foreach (BattleEntity entity in _entitiesInArea)
        {
            _damageDealt += Mathf.RoundToInt(entity.ArmyEntity.CalculateDamage(_selectedAbility));
            StartCoroutine(entity.GetHit(_selectedAbility));
        }
        CreateBattleLog();

        yield return new WaitForSeconds(3f);
        yield return _effectInstance.transform.DOScale(0f, 1f)
                 .SetEase(Ease.OutCubic)
                 .WaitForCompletion();

        CancelAbility();
    }

    public override void CancelAbilityHighlight()
    {
        RotateSprite rs = _areaHighlightInstance.GetComponentInChildren<RotateSprite>();
        if (rs != null) DOTween.Kill(rs.transform);

        base.CancelAbilityHighlight();

    }
}

