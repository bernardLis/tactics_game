using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleCreatureAbilityPoisonBite : BattleCreatureAbility
    {
        [SerializeField] GameObject _effect;

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            if (!BattleCreature.IsOpponentInRange()) yield break;

            yield return transform.DODynamicLookAt(BattleCreature.Opponent.transform.position, 0.2f, AxisConstraint.Y);
            Animator.SetTrigger(AnimAbility);

            _effect.transform.parent = BattleCreature.Opponent.transform;
            _effect.transform.localPosition = Vector3.one;
            _effect.SetActive(true);
            StartCoroutine(BattleCreature.Opponent.GetPoisoned(BattleCreature));

            Invoke(nameof(CleanUp), 2f);
            yield return base.ExecuteAbilityCoroutine();
        }

        void CleanUp()
        {
            _effect.SetActive(false);
        }
    }
}