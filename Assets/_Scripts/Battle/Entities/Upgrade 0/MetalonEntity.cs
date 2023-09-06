using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MetalonEntity : BattleCreatureMelee
{
    [SerializeField] float _abilityEffectRadius = 5f;

    protected override IEnumerator Attack()
    {
        yield return ManageCreatureAbility();
        yield return base.Attack();
    }

    protected override IEnumerator CreatureAbility()
    {
        if (!IsOpponentInRange()) yield break;

        yield return transform.DODynamicLookAt(Opponent.transform.position, 0.2f, AxisConstraint.Y);
        yield return base.CreatureAbility();
        _currentAttackCooldown = Creature.AttackCooldown.GetValue();

        Collider[] colliders = Physics.OverlapSphere(transform.position, _abilityEffectRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out BattleEntity entity))
            {
                if (entity.Team == Team) continue;
                if (entity.IsDead) continue;

                entity.DisplayFloatingText("Taunted", Color.red);
                entity.Engage(this);
            }
        }
    }

}
