using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleCreatureAbilityWhirlwind : BattleCreatureAbility
    {
        readonly float _radius = 5f;
        [SerializeField] GameObject _effect;

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            if (!BattleCreature.IsOpponentInRange()) yield break;
            _effect.SetActive(true);

            Animator.SetTrigger(AnimAbility);

            foreach (BattleEntity be in GetOpponentsInRadius(_radius))
            {
                StartCoroutine(be.GetHit(BattleCreature,
                    Mathf.FloorToInt(Creature.Power.GetValue() * 2)));
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