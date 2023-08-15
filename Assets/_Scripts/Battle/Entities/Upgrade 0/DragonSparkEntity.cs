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
        if (!IsOpponentInRange()) yield break;

        yield return transform.DODynamicLookAt(Opponent.transform.position, 0.2f).WaitForCompletion();
        yield return base.CreatureAbility();
        _currentAttackCooldown = Creature.AttackCooldown;

        GameObject projectileInstance = Instantiate(_specialProjectile, _projectileSpawnPoint.transform.position, Quaternion.identity);
        projectileInstance.transform.parent = _GFX.transform;
        projectileInstance.GetComponent<Projectile>().Shoot(this, Opponent, Creature.GetPower());
    }
}
