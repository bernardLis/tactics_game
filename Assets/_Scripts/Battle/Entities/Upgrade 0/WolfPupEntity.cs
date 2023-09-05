using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WolfPupEntity : BattleCreatureMelee
{
    [SerializeField] GameObject _effect;
    GameObject _effectInstance;

    //TODO: I'd prefer if it used its ability whenever it is off cooldown, it is not shielded and ability is available
    protected override IEnumerator Attack()
    {
        yield return ManageCreatureAbility();
        yield return base.Attack();
    }

    protected override IEnumerator PathToOpponent()
    {
        yield return ManageCreatureAbility();
        yield return base.PathToOpponent();
    }

    protected override IEnumerator CreatureAbility()
    {

        yield return transform.DODynamicLookAt(Opponent.transform.position, 0.2f, AxisConstraint.Y).WaitForCompletion();
        yield return base.CreatureAbility();
        _currentAttackCooldown = Creature.AttackCooldown;

        _effectInstance = Instantiate(_effect, transform.position, Quaternion.identity);
        _effectInstance.transform.parent = transform;

        Vector3 normal = (Opponent.transform.position - transform.position).normalized;
        Vector3 targetPosition = transform.position + normal * 10f;

        // if opp is in range, jump behind him not *10f
        if (IsOpponentInRange())
        {
            targetPosition = transform.position + normal * (_agent.stoppingDistance * 2);
            StartCoroutine(Opponent.GetHit(this, Creature.GetPower() * 3));
        }
        transform.DOJump(targetPosition, 2f, 1, 0.3f, false);

        Invoke(nameof(CleanUp), 2f);
    }

    void CleanUp()
    {
        if (_effectInstance != null)
            Destroy(_effectInstance);
    }

}
