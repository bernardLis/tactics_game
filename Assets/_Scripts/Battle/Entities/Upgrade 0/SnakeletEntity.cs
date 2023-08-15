using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Snakelet : BattleCreatureMelee
{

    [SerializeField] GameObject _specialHit;
    GameObject _specialHitInstance;

    protected override IEnumerator Attack()
    {
        yield return ManageCreatureAbility();
        yield return base.Attack();
    }

    protected override IEnumerator CreatureAbility()
    {
        if (!IsOpponentInRange()) yield break;

        yield return transform.DODynamicLookAt(Opponent.transform.position, 0.2f, AxisConstraint.Y);
        yield return base.CreatureAbility();
        _currentAttackCooldown = Creature.AttackCooldown;

        _specialHitInstance = Instantiate(_specialHit, Opponent.transform.position, Quaternion.identity);
        _specialHitInstance.transform.parent = Opponent.transform;
        StartCoroutine(Opponent.GetPoisoned(this));

        Invoke(nameof(CleanUp), 2f);
    }

    void CleanUp()
    {
        if (_specialHitInstance != null)
            Destroy(_specialHitInstance);
    }
}
