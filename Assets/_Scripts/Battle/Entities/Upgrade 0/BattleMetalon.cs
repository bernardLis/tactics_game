using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleMetalon : BattleCreatureMelee
    {
        [SerializeField] float _abilityEffectRadius = 5f;
        [SerializeField] GameObject _tauntEffect;

        // protected override IEnumerator Attack()
        // {
        //     yield return ManageCreatureAbility();
        //     yield return base.Attack();
        // }
        //
        // protected override IEnumerator CreatureAbility()
        // {
        //     if (!IsOpponentInRange()) yield break;
        //
        //     yield return transform.DODynamicLookAt(Opponent.transform.position, 0.2f, AxisConstraint.Y);
        //     yield return base.CreatureAbility();
        //     CurrentAttackCooldown = Creature.AttackCooldown.GetValue();
        //     _tauntEffect.SetActive(true);
        //
        //     Collider[] colliders = new Collider[10];
        //     Physics.OverlapSphereNonAlloc(transform.position, _abilityEffectRadius, colliders);
        //     foreach (Collider c in colliders)
        //     {
        //         if (!c.TryGetComponent(out BattleEntity entity)) continue;
        //         if (entity.Team == Team) continue;
        //         if (entity.IsDead) continue;
        //
        //         entity.DisplayFloatingText("Taunted", Color.red);
        //         entity.GetEngaged(this);
        //     }
        //
        //     yield return new WaitForSeconds(2f);
        //     _tauntEffect.SetActive(false);
        // }
    }
}