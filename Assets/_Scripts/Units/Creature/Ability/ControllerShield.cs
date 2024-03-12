using System.Collections;
using Lis.Core;
using UnityEngine;

namespace Lis.Units.Creature.Ability
{
    public class ControllerShield : Controller
    {
        [SerializeField] GameObject _effect;

        Color _shieldedColor;

        public override void Initialize(CreatureController creatureController)
        {
            base.Initialize(creatureController);

            CreatureController.OnDeath += (_, _) => { _effect.SetActive(false); };

            _shieldedColor = GameManager.Instance.GameDatabase.GetColorByName("Water").Primary;
            CreatureController.OnShieldBroken += BreakShield;
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            if (CreatureController.IsShielded || CreatureController.IsDead)
            {
                yield return base.ExecuteAbilityCoroutine();
                yield break;
            }

            Animator.SetTrigger(AnimAbility);
            AudioManager.PlaySfx(Ability.Sound, transform.position);

            CreatureController.DisplayFloatingText("Shielded", _shieldedColor);
            _effect.SetActive(true);
            CreatureController.IsShielded = true;

            yield return base.ExecuteAbilityCoroutine();
        }

        void BreakShield()
        {
            _effect.SetActive(false);
        }
    }
}