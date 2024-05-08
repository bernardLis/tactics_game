using Lis.Core;
using UnityEngine;

namespace Lis.Units.Attack
{
    [CreateAssetMenu(menuName = "ScriptableObject/Units/Attacks/Attack")]
    public class Attack : BaseScriptableObject
    {
        public Sprite Icon;
        public string Description;

        public Nature Nature;
        public float Damage;
        public float Range;
        public float Cooldown;
        public bool IsArmorPiercing;

        public Sound Sound;
        [HideInInspector] public int DamageDealt;

        public AttackController AttackControllerPrefab;
        [HideInInspector] public AttackController AttackController;
        UnitController _unitController;
        Unit _unit;

        public void InitializeAttack(UnitController unitController)
        {
            _unitController = unitController;
            _unit = unitController.Unit;
            AttackController = Instantiate(AttackControllerPrefab, _unitController.transform);
            AttackController.Initialize(_unitController, this);
        }

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