using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis.Units.Creature.Ability
{
    public class ControllerTaunt : Controller
    {
        const float _radius = 5f;
        [SerializeField] GameObject _effect;

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            if (!AttackController.IsOpponentInRange()) yield break;

            yield return transform.DODynamicLookAt(CreatureController.Opponent.transform.position, 0.2f,
                AxisConstraint.Y);
            _effect.SetActive(true);
            Animator.SetTrigger(AnimAbility);
            AudioManager.PlaySfx(Ability.Sound, transform.position);

            foreach (UnitController be in GetOpponentsInRadius(_radius))
            {
                be.DisplayFloatingText("Taunted", Color.red);
                be.GetEngaged(CreatureController);
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