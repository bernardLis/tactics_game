using System.Collections;
using UnityEngine;
using DG.Tweening;

public class BattleCreatureMelee : BattleCreature
{
    GameObject _hitInstance;

    protected override IEnumerator PathToOpponent()
    {
        yield return base.PathToOpponent();
        Opponent.Engage(this); // otherwise, creature can't catch up
    }

    protected override IEnumerator Attack()
    {
        EntityLog.Add($"{Time.time}: Melee attack is called");

        while (!CanAttack()) yield return null;
        _currentAttackCooldown = Creature.AttackCooldown;

        if (_hasSpecialAttack & CanUseSpecialAbility())
        {
            yield return SpecialAbility();
            yield return base.Attack();
            yield break;
        }

        if (!IsOpponentInRange())
        {
            StartRunEntityCoroutine();
            yield break;
        }

        if (_attackSound != null) _audioManager.PlaySFX(_attackSound, transform.position);

        transform.DODynamicLookAt(Opponent.transform.position, 0.2f, AxisConstraint.Y);
        Animator.SetTrigger("Attack");

        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);

        Quaternion q = Quaternion.Euler(0, -90, 0); // HERE: face camera
        _hitInstance = Instantiate(Creature.HitPrefab, Opponent.Collider.bounds.center, q);
        _hitInstance.transform.parent = Opponent.transform;

        yield return Opponent.GetHit(this);
        Invoke(nameof(DestroyHitInstance), 2f);

        yield return base.Attack();
    }

    void DestroyHitInstance()
    {
        Destroy(_hitInstance);
    }

}
