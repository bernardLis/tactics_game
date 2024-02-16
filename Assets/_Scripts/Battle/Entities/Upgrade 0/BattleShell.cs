using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleShell : BattleCreatureMelee
    {
        [SerializeField] GameObject _shieldEffect;
        GameObject _shieldEffectInstance;

        //TODO: it is not an ideal approach
        // I'd prefer if shell used its ability whenever it is off cooldown, it is not shielded and ability is available
        protected override IEnumerator Attack()
        {
            if (!IsShielded)
                yield return ManageCreatureAbility();
            yield return base.Attack();
        }

        protected override IEnumerator PathToOpponent()
        {
            if (!IsShielded)
                yield return ManageCreatureAbility();
            yield return base.PathToOpponent();
        }

        void OnDisabled()
        {
            if (_shieldEffectInstance != null)
                Destroy(_shieldEffectInstance);
        }

        protected override IEnumerator CreatureAbility()
        {
            if (IsShielded) yield break;

            yield return base.CreatureAbility();

            DisplayFloatingText("Shielded", Color.blue);
            _shieldEffect.SetActive(true);
            IsShielded = true;
        }

        public override IEnumerator GetHit(Ability ability)
        {
            if (IsShielded)
            {
                BreakShield();
                yield break;
            }

            yield return base.GetHit(ability);
        }

        public override IEnumerator GetHit(BattleEntity attacker, int specialDamage = 0)
        {
            if (IsShielded)
            {
                BreakShield();
                yield break;
            }

            yield return base.GetHit(attacker, specialDamage);
        }

        void BreakShield()
        {
            DisplayFloatingText("Shield Broken", Color.blue);
            IsShielded = false;
            _shieldEffect.SetActive(false);
        }
    }
}