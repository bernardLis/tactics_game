using System.Collections;
using UnityEngine;
using DG.Tweening;

public class BattleEntityMelee : BattleEntity
{
    GameObject _hitInstance;

    protected override IEnumerator Attack()
    {
        while (!CanAttack()) yield return null;

        if (_hasSpecialAttack & CurrentSpecialAbilityCooldown <= 0)
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

        transform.DODynamicLookAt(_opponent.transform.position, 0.2f, AxisConstraint.Y);
        Animator.SetTrigger("Attack");

        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);

        Quaternion q = Quaternion.Euler(0, -90, 0); // face default camera position
        _hitInstance = Instantiate(Creature.HitPrefab, _opponent.Collider.bounds.center, q);
        _hitInstance.transform.parent = _opponent.transform;

        yield return _opponent.GetHit(this);
        Invoke("DestroyHitInstance", 2f);

        yield return base.Attack();
    }

    void DestroyHitInstance()
    {
        Destroy(_hitInstance);
    }

}
