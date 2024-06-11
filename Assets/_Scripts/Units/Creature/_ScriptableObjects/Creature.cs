using System;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Upgrades;
using UnityEngine;

namespace Lis.Units.Creature
{
    [CreateAssetMenu(menuName = "ScriptableObject/Units/Creature/Creature")]
    public class Creature : Unit
    {
        [Header("Creature")] public int UpgradeTier;

        public Attack.Attack SpecialAttack;

        public override void InitializeBattle(int team)
        {
            base.InitializeBattle(team);
            UpgradeBoard globalUpgradeBoard = GameManager.Instance.UpgradeBoard;
            MaxHealth.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Creature Health").GetValue());
            CurrentHealth.SetValue(MaxHealth.GetValue());
            Armor.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Creature Armor").GetValue());
            Speed.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Creature Speed").GetValue());
            Power.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Creature Power").GetValue());

            for (int i = 0; i < globalUpgradeBoard.GetUpgradeByName("Creature Level").GetValue(); i++)
                LevelUp();

            if (UnitName.Length == 0) UnitName = Helpers.ParseScriptableObjectName(name);

            SpecialAttack = Instantiate(SpecialAttack);
        }

        public bool IsAbilityUnlocked()
        {
            return Level.Value >= 6;
        }

        public override void AddKill(Unit unit)
        {
            base.AddKill(unit);
            AddExp(unit.Price);
        }

        public event Action OnDeath;

        public void Die()
        {
            OnDeath?.Invoke();
        }

        public new CreatureData SerializeSelf()
        {
            // TODO: needs to be implemented
            CreatureData data = new()
            {
                CreatureId = Id,

                Name = UnitName,
                Level = Level.Value,

                KillCount = TotalKillCount
            };

            return data;
        }

        public void LoadFromData(CreatureData data)
        {
            UnitName = data.Name;

            Level = CreateInstance<IntVariable>();
            Level.SetValue(data.Level);

            TotalKillCount = data.KillCount;
        }
    }

    [Serializable]
    public struct CreatureData
    {
        public string Name;
        public int Level;
        public string CreatureId;

        public int KillCount;
        public int DamageDealt;
        public int DamageTaken;
    }
}