using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DragonSparkEntity : BattleEntityRanged
{
    [SerializeField] GameObject _specialProjectile;

    protected override void Start()
    {
        _hasSpecialAttack = true;
        base.Start();
    }

    protected override IEnumerator SpecialAbility()
    {
        if (!IsOpponentInRange())
        {
            StartRunEntityCoroutine();
            yield break;
        }

        yield return transform.DODynamicLookAt(_opponent.transform.position, 0.2f).WaitForCompletion();

        if (_specialAbilitySound != null) _audioManager.PlaySFX(_specialAbilitySound, transform.position);

        Animator.SetTrigger("Special Attack");
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f);
        GameObject projectileInstance = Instantiate(_specialProjectile, _projectileSpawnPoint.transform.position, Quaternion.identity);
        projectileInstance.transform.parent = _GFX.transform;
        projectileInstance.GetComponent<Projectile>().Shoot(this, _opponent, Creature.GetPower());

        yield return base.SpecialAbility();
    }
}
