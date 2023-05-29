using System.Collections;
using UnityEngine;
using DG.Tweening;

public class BattleEntityMelee : BattleEntity
{
    GameObject _hitInstance;

    protected override IEnumerator Attack()
    {
        while (!CanAttack()) yield return null;

        if (_hasSpecialAttack & _currentSpecialAbilityCooldown <= 0)
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

        Debug.Log($"in attack after current special ability check");

        transform.DODynamicLookAt(_opponent.transform.position, 0.2f, AxisConstraint.Y);
        Animator.SetTrigger("Attack");

        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);

        Quaternion q = Quaternion.Euler(0, -90, 0); // face default camera position
        _hitInstance = Instantiate(ArmyEntity.HitPrefab, _opponent.Collider.bounds.center, q);

        yield return _opponent.GetHit(this);
        Invoke("DestroyHitInstance", 2f);

        yield return base.Attack();
    }

    void DestroyHitInstance()
    {
        Destroy(_hitInstance);
    }

}
