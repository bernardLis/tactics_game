using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FireballExecutor : AbilityExecutor
{
    public override void ExecuteAbility(Ability ability)
    {
        base.ExecuteAbility(ability);

        ParticleSystem ps = _effectInstance.GetComponent<ParticleSystem>();
        var shape = ps.shape;
        float originalRadius = shape.radius;
        shape.radius = originalRadius * _selectedAbility.GetScale();

        var burst = ps.emission.GetBurst(0);
        burst.count = ability.Level + 1;
        ps.emission.SetBurst(0, burst);
    }

    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        Debug.Log($"Executing fireball on {_entitiesInArea.Count}");
        foreach (BattleEntity entity in _entitiesInArea)
        {
            _damageDealt += Mathf.RoundToInt(entity.Entity.CalculateDamage(_selectedAbility));
            StartCoroutine(entity.GetHit(_selectedAbility));
        }

        yield return new WaitForSeconds(6f);


        CancelAbility();
    }

}
