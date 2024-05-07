using Lis.Core;
using UnityEngine;

namespace Lis.Units.Attack
{
    [CreateAssetMenu(menuName = "ScriptableObject/Units/Attacks/Attack")]
    public class Attack : BaseScriptableObject
    {
        public Nature Nature;
        public float Damage;
        public float Range;
        public float Cooldown;
        public bool IsArmorPiercing;

        [HideInInspector] public int DamageDealt;

        Unit _unit;

        public void InitializeAttack(Unit unit)
        {
            _unit = unit;
        }

        public float GetDamage()
        {
            return Damage + _unit.Power.GetValue();
        }

        public void AddDamageDealt(int dmg)
        {
            DamageDealt += dmg;
        }

        public void AddKill(Unit killedUnit)
        {
            _unit.AddKill(killedUnit);
        }
    }
}