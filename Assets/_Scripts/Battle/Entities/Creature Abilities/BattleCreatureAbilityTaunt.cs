using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleCreatureAbilityTaunt : BattleCreatureAbility
    {
        const float _radius = 5f;
        [SerializeField] GameObject _effect;

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            if (!BattleCreature.IsOpponentInRange()) yield break;

            yield return transform.DODynamicLookAt(BattleCreature.Opponent.transform.position, 0.2f, AxisConstraint.Y);
            _effect.SetActive(true);
            Animator.SetTrigger(AnimAbility);

            foreach (BattleEntity be in GetOpponentsInRadius(_radius))
            {
                be.DisplayFloatingText("Taunted", Color.red);
                be.GetEngaged(BattleCreature);
            }

            Invoke(nameof(CleanUp), 2f);
            yield return base.ExecuteAbilityCoroutine();
        }

        void CleanUp()
        {
            _effect.SetActive(false);
        }
    }
}