using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleWolfPup : BattleCreatureMelee
    {
        [Header("Wolf Pup")]
        [SerializeField] Transform _pickupPosition;
        BattlePickup _currentPickup;

        [SerializeField] GameObject _effect;
        GameObject _effectInstance;

        protected override IEnumerator HangOutCoroutine()
        {
            yield return new WaitForSeconds(Random.Range(2f, 4f));

            // while (_battleManager.Pickups.Count > 0)
            // {
            //     // woof checks the list picks a random pickup
            //     _currentPickup = _battleManager.Pickups[Random.Range(0, _battleManager.Pickups.Count)];
            //     // goes there,
            //     Vector3 pickupPos = _currentPickup.transform.position;

            //     yield return PathToPositionAndStop(pickupPos);

            //     // check if it is still there
            //     if (_currentPickup == null) continue;
            //     if (pickupPos != _currentPickup.transform.position) continue;

            //     // takes it
            //     Animator.SetTrigger("Attack");
            //     _currentPickup.transform.parent = _pickupPosition;
            //     _currentPickup.transform.localPosition = new Vector3(-0.3f, -0.2f, 0);
            //     yield return new WaitForSeconds(0.4f);

            //     // and brings it to hero
            //     BattleHero battleHero = _battleManager.GetComponent<BattleHeroManager>().BattleHero;
            //     _agent.stoppingDistance = 4f;
            //     yield return PathToTarget(battleHero.transform);

            //     yield return new WaitForSeconds(Random.Range(3f, 6f));
            // }

            yield return base.HangOutCoroutine();
        }

        void DropPickup()
        {
            _currentPickup.transform.parent = BattleManager.EntityHolder;
            _currentPickup.transform.position = transform.position;
        }

        //TODO: I'd prefer if it used its ability whenever it is off cooldown, it is not shielded and ability is available
        protected override IEnumerator Attack()
        {
            yield return ManageCreatureAbility();
            yield return base.Attack();
        }

        protected override IEnumerator PathToOpponent()
        {
            if (_currentPickup != null) DropPickup();

            yield return ManageCreatureAbility();
            yield return base.PathToOpponent();
        }

        protected override IEnumerator CreatureAbility()
        {

            yield return transform.DODynamicLookAt(Opponent.transform.position, 0.2f, AxisConstraint.Y).WaitForCompletion();
            yield return base.CreatureAbility();
            CurrentAttackCooldown = Creature.AttackCooldown.GetValue();

            _effectInstance = Instantiate(_effect, transform.position, Quaternion.identity);
            _effectInstance.transform.parent = transform;

            Vector3 normal = (Opponent.transform.position - transform.position).normalized;
            Vector3 targetPosition = transform.position + normal * 10f;

            // if opp is in range, jump behind him not *10f
            if (IsOpponentInRange())
            {
                targetPosition = transform.position + normal * (Agent.stoppingDistance * 2);
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
}
