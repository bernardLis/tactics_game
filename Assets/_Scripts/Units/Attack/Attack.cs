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

        public AttackController AttackControllerPrefab;
        [HideInInspector] public AttackController AttackController;
        UnitController _unitController;

        public void InitializeAttack(UnitController unitController)
        {
            _unitController = unitController;
            AttackController = Instantiate(AttackControllerPrefab, unitController.transform);
            AttackController.Initialize(unitController, this);
        }

        public float GetDamage()
        {
            return Damage + _unitController.Unit.Power.GetValue();
        }

        public void AddDamageDealt(int dmg)
        {
            DamageDealt += dmg;
        }

        public void AddKill(Unit killedUnit)
        {
            _unitController.Unit.AddKill(killedUnit);
        }
    }
}