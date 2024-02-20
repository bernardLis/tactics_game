using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleWolfPup : BattleCreatureMelee
    {
        [SerializeField] GameObject _effect;

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

            _effect.SetActive(true);

            Vector3 normal = (oppPosition - transformPosition).normalized;
            Vector3 targetPosition = transformPosition + normal * 10f;

            // if opp is in range, jump behind him not *10f
            if (IsOpponentInRange())
            {
                targetPosition = transform.position + normal * (Creature.AttackRange.GetValue() * 2);
                StartCoroutine(Opponent.GetHit(this, Mathf.FloorToInt(Creature.Power.GetValue() * 3)));
            }

            Collider.enabled = false;
            targetPosition.y = 1;
            transform.DOJump(targetPosition, 2f, 1, 0.3f)
                .OnComplete(() => Collider.enabled = true);

            Invoke(nameof(CleanUp), 2f);
        }

        void CleanUp()
        {
            _effect.SetActive(false);
        }
    }
}