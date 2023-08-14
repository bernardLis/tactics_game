using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DragonSparkEntity : BattleCreatureRanged
{
    [SerializeField] GameObject _specialProjectile;

    protected override IEnumerator Attack()
    {
        yield return ManageCreatureAbility();
        yield return base.Attack();
    }

    protected override IEnumerator CreatureAbility()
    {
        Debug.Log($"ability");
        if (!IsOpponentInRange())
            yield break;

        yield return base.CreatureAbility();
        _currentAttackCooldown = Creature.AttackCooldown;

        yield return transform.DODynamicLookAt(Opponent.transform.position, 0.2f).WaitForCompletion();

        if (_creatureAbilitySound != null)
            _audioManager.PlaySFX(_creatureAbilitySound, transform.position);

        Animator.SetTrigger("Special Attack");
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f);
        GameObject projectileInstance = Instantiate(_specialProjectile, _projectileSpawnPoint.transform.position, Quaternion.identity);
        projectileInstance.transform.parent = _GFX.transform;
        projectileInstance.GetComponent<Projectile>().Shoot(this, Opponent, Creature.GetPower());

    }
}
