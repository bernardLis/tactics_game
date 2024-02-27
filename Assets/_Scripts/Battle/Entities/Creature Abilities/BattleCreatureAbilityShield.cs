using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleCreatureAbilityShield : BattleCreatureAbility
    {
        [SerializeField] GameObject _effect;
        GameObject _effectInstance;

        Color _shieldedColor;

        public override void Initialize(BattleCreature battleCreature)
        {
            base.Initialize(battleCreature);

            BattleCreature.OnDeath += (_, _) =>
            {
                if (_effectInstance != null)
                    Destroy(_effectInstance);
            };

            _shieldedColor = GameManager.Instance.GameDatabase.GetColorByName("Water").Primary;
            _effectInstance = Instantiate(_effect, transform);
            BattleCreature.OnHit += BreakShield;
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            if (BattleCreature.IsShielded) yield break;

            BattleCreature.DisplayFloatingText("Shielded", _shieldedColor);
            _effectInstance.SetActive(true);
            BattleCreature.IsShielded = true;

            yield return base.ExecuteAbilityCoroutine();
        }

        void BreakShield()
        {
            if (!BattleCreature.IsShielded) return;

            BattleCreature.DisplayFloatingText("Shield broken", _shieldedColor);
            _effectInstance.SetActive(false);
            BattleCreature.IsShielded = false;
        }
    }
}