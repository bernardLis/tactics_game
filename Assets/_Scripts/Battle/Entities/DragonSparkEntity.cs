using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DragonSparkEntity : BattleEntityRanged
{
    [SerializeField] GameObject _specialProjectile;

    protected override IEnumerator SpecialAbility()
    {
        Debug.Log($"in special ability");

        if (!IsOpponentInRange())
        {
            StartRunEntityCoroutine();
            yield break;
        }

        yield return transform.DODynamicLookAt(_opponent.transform.position, 0.2f).WaitForCompletion();
        Animator.SetTrigger("Special Attack");

        // HERE: projectile spawning and animation delay per entity
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f);
        GameObject projectileInstance = Instantiate(_specialProjectile, _projectileSpawnPoint.transform.position, Quaternion.identity);
        projectileInstance.transform.parent = _GFX.transform;

        // HERE: projectile speed
        projectileInstance.GetComponent<Projectile>().Shoot(this, _opponent, 20, ArmyEntity.Power);

        yield return base.SpecialAbility();
    }
}
