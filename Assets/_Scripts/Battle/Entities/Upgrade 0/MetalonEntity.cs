using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MetalonEntity : BattleCreatureMelee
{
    [SerializeField] float _specialEffectRadius = 5f;

    protected override IEnumerator Attack()
    {
        yield return CreatureAbility();
        yield return base.Attack();
    }

    protected override IEnumerator CreatureAbility()
    {
        if (!IsOpponentInRange())
            yield break;

        Animator.SetTrigger("Special Attack");
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);

        if (_creatureAbilitySound != null) _audioManager.PlaySFX(_creatureAbilitySound, transform.position);

        Collider[] colliders = Physics.OverlapSphere(transform.position, _specialEffectRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<BattleEntity>(out BattleEntity entity))
            {
                if (entity.Team == Team) continue;
                if (entity.IsDead) continue;

                entity.DisplayFloatingText("Taunted", Color.red);

                if (entity is BattleCreature creature)
                    creature.SetOpponent(this);
                if (entity is BattleMinion minion)
                    StartCoroutine(entity.GetHit(this, Mathf.RoundToInt(Creature.GetPower() * 0.5f)));
            }
        }

        _currentAttackCooldown = Creature.AttackCooldown;

        yield return base.CreatureAbility();
    }

}
