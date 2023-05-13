using System.Collections;
using UnityEngine;

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

        _animator.SetTrigger("Attack");

        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);

        _currentAttackCooldown = ArmyEntity.AttackCooldown;
        Quaternion q = Quaternion.Euler(0, -90, 0); // face default camera position
        GameObject hitInstance = Instantiate(ArmyEntity.HitPrefab, _opponent.Collider.bounds.center, q);

        yield return _opponent.GetHit(this);
        Destroy(hitInstance);

        yield return base.Attack();
    }

}
