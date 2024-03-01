using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis.Units.Creature.Ability
{
    public class ControllerDash : Controller
    {
        [SerializeField] GameObject _effect;

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            if (CreatureController.Opponent == null || CreatureController.IsDead)
            {
                yield return base.ExecuteAbilityCoroutine();
                yield break;
            }

            CreatureController.UnitLog.Add(
                $"{BattleManager.GetTime()}: Unit uses ability {Creature.Ability.name}");

            CreatureController.StopUnit();

            Vector3 transformPosition = transform.position;
            Vector3 oppPosition = CreatureController.Opponent.transform.position;
            yield return CreatureController.transform.DODynamicLookAt(oppPosition, 0.2f, AxisConstraint.Y)
                .WaitForCompletion();

            _effect.SetActive(true);

            Vector3 normal = (oppPosition - transformPosition).normalized;
            Vector3 targetPosition = transformPosition + normal * 10f;

            // if opp is in range, jump behind him not *10f
            if (CreatureController.IsOpponentInRange())
            {
                targetPosition = transform.position + normal * (Creature.AttackRange.GetValue() * 2);
                StartCoroutine(CreatureController.Opponent.GetHit(CreatureController,
                    Mathf.FloorToInt(Creature.Power.GetValue() * 3)));
            }

            Collider.enabled = false;
            targetPosition.y = 1;
            Animator.SetTrigger(AnimAbility);
            AudioManager.PlaySFX(Ability.Sound, transform.position);
            CreatureController.transform.DOJump(targetPosition, 2f, 1, 0.3f)
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