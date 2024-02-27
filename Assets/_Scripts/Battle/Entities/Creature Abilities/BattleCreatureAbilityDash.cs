using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleCreatureAbilityDash : BattleCreatureAbility
    {
        [SerializeField] GameObject _effect;

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            BattleCreature.StopRunEntityCoroutine();

            Vector3 transformPosition = transform.position;
            Vector3 oppPosition = BattleCreature.Opponent.transform.position;
            yield return transform.DODynamicLookAt(oppPosition, 0.2f, AxisConstraint.Y).WaitForCompletion();

            _effect.SetActive(true);

            Vector3 normal = (oppPosition - transformPosition).normalized;
            Vector3 targetPosition = transformPosition + normal * 10f;

            // if opp is in range, jump behind him not *10f
            if (BattleCreature.IsOpponentInRange())
            {
                targetPosition = transform.position + normal * (Creature.AttackRange.GetValue() * 2);
                StartCoroutine(BattleCreature.Opponent.GetHit(BattleCreature,
                    Mathf.FloorToInt(Creature.Power.GetValue() * 3)));
            }

            Collider.enabled = false;
            targetPosition.y = 1;
            Animator.SetTrigger(AnimAbility);
            transform.DOJump(targetPosition, 2f, 1, 0.3f)
                .OnComplete(() => Collider.enabled = true);

            Invoke(nameof(CleanUp), 2f);
            yield return base.ExecuteAbilityCoroutine();
        }

        void CleanUp()
        {
            _effect.SetActive(false);
        }
    }
}