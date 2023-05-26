using System.Collections;
using UnityEngine;
using DG.Tweening;

public class BattleEntityMelee : BattleEntity
{

    protected override IEnumerator Attack()
    {
        while (!CanAttack()) yield return null;
        if (!IsOpponentInRange())
        {
            StartRunEntityCoroutine();
            yield break;
        }

        transform.DODynamicLookAt(_opponent.transform.position, 0.2f, AxisConstraint.Y);
        Animator.SetTrigger("Attack");

        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);

        _currentAttackCooldown = ArmyEntity.AttackCooldown;
        Quaternion q = Quaternion.Euler(0, -90, 0); // face default camera position
        GameObject hitInstance = Instantiate(ArmyEntity.HitPrefab, _opponent.Collider.bounds.center, q);

        yield return _opponent.GetHit(this);
        Destroy(hitInstance);

        yield return base.Attack();
    }

}
