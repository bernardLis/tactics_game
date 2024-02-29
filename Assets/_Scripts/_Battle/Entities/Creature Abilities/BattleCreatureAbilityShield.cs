using System.Collections;
using Lis.Core;
using UnityEngine;

namespace Lis
{
    public class BattleCreatureAbilityShield : BattleCreatureAbility
    {
        [SerializeField] GameObject _effect;

        Color _shieldedColor;

        public override void Initialize(BattleCreature battleCreature)
        {
            base.Initialize(battleCreature);

            BattleCreature.OnDeath += (_, _) => { _effect.SetActive(false); };

            _shieldedColor = GameManager.Instance.GameDatabase.GetColorByName("Water").Primary;
            BattleCreature.OnShieldBroken += BreakShield;
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            Debug.Log($"Trying to shield is already shielded? {BattleCreature.IsShielded}");
            if (BattleCreature.IsShielded || BattleCreature.IsDead)
            {
                yield return base.ExecuteAbilityCoroutine();
                yield break;
            }

            Animator.SetTrigger(AnimAbility);
            AudioManager.PlaySFX(CreatureAbility.Sound, transform.position);

            BattleCreature.DisplayFloatingText("Shielded", _shieldedColor);
            _effect.SetActive(true);
            BattleCreature.IsShielded = true;

            yield return base.ExecuteAbilityCoroutine();
        }

        void BreakShield()
        {
            _effect.SetActive(false);
        }
    }
}