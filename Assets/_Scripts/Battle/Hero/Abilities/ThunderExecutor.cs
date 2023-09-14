using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ThunderExecutor : AbilityExecutor
{
    public override void ExecuteAbility(Ability ability)
    {
        base.ExecuteAbility(ability);

        _effectInstance.transform.localScale = Vector3.one * _selectedAbility.GetScale();
    }

    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        //   Debug.Log($"Executing thunder on {_entitiesInArea.Count}");
        List<GameObject> entityEffects = new();
        foreach (BattleEntity entity in _entitiesInArea)
        {
            if (entity.IsDead) continue;
            if (entity == null) continue;

            _damageDealt += Mathf.RoundToInt(entity.Entity.CalculateDamage(_selectedAbility));
            StartCoroutine(entity.GetHit(_selectedAbility));
            Vector3 pos = new Vector3(entity.transform.position.x, 0, entity.transform.position.z);
            GameObject instance = Instantiate(_entityEffectPrefab, pos, Quaternion.identity);
            entityEffects.Add(instance);
        }

        yield return new WaitForSeconds(3f);
        foreach (GameObject g in entityEffects)
            Destroy(g);

        CancelAbility();
    }

    public override void ClearAbilityHighlight()
    {
        if (_areaHighlightInstance == null) return;
        RotateSprite rs = _areaHighlightInstance.GetComponentInChildren<RotateSprite>();
        if (rs != null) DOTween.Kill(rs.transform);

        base.ClearAbilityHighlight();
    }


    public override void CancelAbilityHighlight()
    {
        RotateSprite rs = _areaHighlightInstance.GetComponentInChildren<RotateSprite>();
        if (rs != null) DOTween.Kill(rs.transform);

        base.CancelAbilityHighlight();

    }
}

