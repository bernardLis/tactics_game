using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleWolfPup : BattleCreatureMelee
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
            Vector3 transformPosition = transform.position;
            Vector3 oppPosition = Opponent.transform.position;
            yield return transform.DODynamicLookAt(oppPosition, 0.2f, AxisConstraint.Y).WaitForCompletion();
            yield return base.CreatureAbility();
            CurrentAttackCooldown = Creature.AttackCooldown.GetValue();

            _effectInstance = Instantiate(_effect, transformPosition, Quaternion.identity);
            _effectInstance.transform.parent = transform;

            Vector3 normal = (oppPosition - transformPosition).normalized;
            Vector3 targetPosition = transformPosition + normal * 10f;

            // if opp is in range, jump behind him not *10f
            if (IsOpponentInRange())
            {
                targetPosition = transform.position + normal * (Creature.AttackRange.GetValue() * 2);
                StartCoroutine(Opponent.GetHit(this, Creature.Power.GetValue() * 3));
            }

            transform.DOJump(targetPosition, 2f, 1, 0.3f);

            Invoke(nameof(CleanUp), 2f);
        }

        void CleanUp()
        {
            if (_effectInstance != null)
                Destroy(_effectInstance);
        }
    }
}