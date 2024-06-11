using System.Collections;
using Lis.Core;
using UnityEngine;

namespace Lis.Units.Attack
{
    public class AttackControllerShield : AttackController
    {
        [SerializeField] private GameObject _effect;
        private Color _shieldedColor;

        public override void Initialize(UnitController unitController, Attack attack)
        {
            base.Initialize(unitController, attack);
            _shieldedColor = GameManager.Instance.GameDatabase.GetColorByName("Water").Primary;

            UnitController.OnDeath += (_, _) => BreakShield();
            UnitController.OnShieldBroken += BreakShield;
        }

        public override IEnumerator AttackCoroutine()
        {
            while (CurrentCooldown > 0) yield return new WaitForSeconds(0.1f);
            BaseAttack();
            if (UnitController.IsShielded || UnitController.IsDead) yield break;

            Animator.SetTrigger(AnimSpecialAttack);
            AudioManager.PlaySfx(Attack.Sound, transform.position);

            UnitController.DisplayFloatingText("Shielded", _shieldedColor);
            _effect.SetActive(true);
            UnitController.IsShielded = true;
        }

        private void BreakShield()
        {
            _effect.SetActive(false);
        }
    }
}