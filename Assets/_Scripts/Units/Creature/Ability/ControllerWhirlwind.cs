using System.Collections;
using UnityEngine;

namespace Lis.Units.Creature.Ability
{
    public class ControllerWhirlwind : Controller
    {
        readonly float _radius = 5f;
        [SerializeField] GameObject _effect;

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            if (!UnitAttackController.IsOpponentInRange()) yield break;
            _effect.SetActive(true);

            Animator.SetTrigger(AnimAbility);
            AudioManager.PlaySfx(Ability.Sound, transform.position);

            foreach (UnitController be in GetOpponentsInRadius(_radius))
                StartCoroutine(be.GetHit(Creature.Ability.Attack));

            Invoke(nameof(CleanUp), 2f);
            yield return base.ExecuteAbilityCoroutine();
        }

        void CleanUp()
        {
            _effect.SetActive(false);
        }
    }
}