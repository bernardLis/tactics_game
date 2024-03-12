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
            if (!CreatureController.IsOpponentInRange()) yield break;
            _effect.SetActive(true);

            Animator.SetTrigger(AnimAbility);
            AudioManager.PlaySfx(Ability.Sound, transform.position);

            foreach (UnitController be in GetOpponentsInRadius(_radius))
            {
                StartCoroutine(be.GetHit(CreatureController,
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