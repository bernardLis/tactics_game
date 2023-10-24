using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleWolfPup : BattleCreatureMelee
{
    [Header("Wolf Pup")]
    [SerializeField] Transform _pickupPosition;
    BattlePickUp _currentPickup;

    [SerializeField] GameObject _effect;
    GameObject _effectInstance;

    protected override IEnumerator HangOut()
    {
        yield return new WaitForSeconds(Random.Range(2f, 4f));

        while (_battleManager.Pickups.Count > 0)
        {
            // woof checks the list picks a random pickup
            BattlePickUp battlePickUp = _battleManager.Pickups[Random.Range(0, _battleManager.Pickups.Count)];
            // goes there,
            Vector3 pickupPos = battlePickUp.transform.position;
            yield return PathToPosition(pickupPos);
            while (_agent.enabled && _agent.remainingDistance > _agent.stoppingDistance)
                yield return new WaitForSeconds(0.1f);
            Animator.SetBool("Move", false);

            // check if it is still there
            if (battlePickUp == null) continue;
            if (pickupPos != battlePickUp.transform.position) continue;

            // takes it
            Animator.SetTrigger("Attack");
            battlePickUp.transform.parent = _pickupPosition;
            battlePickUp.transform.localPosition = new Vector3(-0.3f, -0.2f, 0);
            yield return new WaitForSeconds(0.4f);

            // and brings it to hero
            BattleHero battleHero = _battleManager.GetComponent<BattleHeroManager>().BattleHero;
            yield return PathToPosition(battleHero.transform.position);
            while (_agent.enabled && _agent.remainingDistance > _agent.stoppingDistance + 2f)
            {
                yield return PathToPosition(battleHero.transform.position);
                yield return new WaitForSeconds(0.1f);
            }
            Animator.SetBool("Move", false);

            yield return new WaitForSeconds(Random.Range(3f, 6f));
        }

        yield return base.HangOut();
    }

    void DropPickup()
    {
        _currentPickup.transform.parent = _battleManager.EntityHolder;
        _currentPickup.transform.localPosition = new Vector3(0f, 0.5f, 0);
    }

    //TODO: I'd prefer if it used its ability whenever it is off cooldown, it is not shielded and ability is available
    protected override IEnumerator Attack()
    {
        yield return ManageCreatureAbility();
        yield return base.Attack();
    }

    protected override IEnumerator PathToOpponent()
    {
        if (_currentPickup != null)
            DropPickup();

        yield return ManageCreatureAbility();
        yield return base.PathToOpponent();
    }

    protected override IEnumerator CreatureAbility()
    {

        yield return transform.DODynamicLookAt(Opponent.transform.position, 0.2f, AxisConstraint.Y).WaitForCompletion();
        yield return base.CreatureAbility();
        _currentAttackCooldown = Creature.AttackCooldown.GetValue();

        _effectInstance = Instantiate(_effect, transform.position, Quaternion.identity);
        _effectInstance.transform.parent = transform;

        Vector3 normal = (Opponent.transform.position - transform.position).normalized;
        Vector3 targetPosition = transform.position + normal * 10f;

        // if opp is in range, jump behind him not *10f
        if (IsOpponentInRange())
        {
            targetPosition = transform.position + normal * (_agent.stoppingDistance * 2);
            StartCoroutine(Opponent.GetHit(Creature, Creature.Power.GetValue() * 3));
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
