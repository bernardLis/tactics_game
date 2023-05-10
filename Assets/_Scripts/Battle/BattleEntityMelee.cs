using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using MoreMountains.Feedbacks;
using System.Linq;

public class BattleEntityMelee : BattleEntity
{
    protected override IEnumerator Attack()
    {
        Debug.Log($"attack before checks");
        while (!CanAttack()) yield return null;
        if (!IsOpponentInRange()) StartRunEntityCoroutine();
        Debug.Log($"attack after checks");

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
