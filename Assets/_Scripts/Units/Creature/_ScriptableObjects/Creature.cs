using System;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Upgrades;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Units.Creature
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Creature")]
    public class Creature : UnitFight
    {
        [Header("Creature")] public int UpgradeTier;

        [FormerlySerializedAs("CreatureAbility")]
        public Ability.Ability Ability;

        public GameObject Projectile;
        public GameObject HitPrefab;

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
        }

        public bool IsAbilityUnlocked()
        {
            return Level.Value >= Ability.UnlockLevel;
        }


        public event Action OnDeath;

        public void Die()
        {
            OnDeath?.Invoke();
        }

        new public CreatureData SerializeSelf()
        {
            // TODO: needs to be implemented
            CreatureData data = new()
            {
                CreatureId = Id,

                Name = UnitName,
                Level = Level.Value,

                KillCount = TotalKillCount,
                DamageDealt = TotalDamageDealt,
                DamageTaken = TotalDamageTaken
            };

            return data;
        }

        public void LoadFromData(CreatureData data)
        {
            UnitName = data.Name;

            Level = CreateInstance<IntVariable>();
            Level.SetValue(data.Level);

            TotalKillCount = data.KillCount;
            TotalDamageDealt = data.DamageDealt;
            TotalDamageTaken = data.DamageTaken;
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