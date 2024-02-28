using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleCreatureAbilityExplode : BattleCreatureAbility
    {
        readonly float _explosionRadius = 5f;
        [SerializeField] GameObject _effect;

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            Animator.SetTrigger(AnimAbility);
            AudioManager.PlaySFX(CreatureAbility.Sound, transform.position);

            _effect.SetActive(true);
            foreach (BattleEntity be in GetOpponentsInRadius(_explosionRadius))
                StartCoroutine(be.GetHit(BattleCreature, 50));

            Invoke(nameof(CleanUp), 2f);
            yield return base.ExecuteAbilityCoroutine();
        }

        void CleanUp()
        {
            _effect.SetActive(false);
        }
    }
}