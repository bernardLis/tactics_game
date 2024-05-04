using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis.Units.Creature.Ability
{
    public class ControllerPoisonBite : Controller
    {
        [SerializeField] GameObject _effect;

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            if (!UnitAttackController.IsOpponentInRange()) yield break;

            yield return transform.DODynamicLookAt(CreatureController.Opponent.transform.position, 0.2f,
                AxisConstraint.Y);
            Animator.SetTrigger(AnimAbility);
            AudioManager.PlaySfx(Ability.Sound, transform.position);

            _effect.transform.parent = CreatureController.Opponent.transform;
            _effect.transform.localPosition = Vector3.one;
            _effect.SetActive(true);
            StartCoroutine(CreatureController.Opponent.GetPoisoned(CreatureController));

            Invoke(nameof(CleanUp), 2f);
            yield return base.ExecuteAbilityCoroutine();
        }

        void CleanUp()
        {
            _effect.SetActive(false);
        }
    }
}