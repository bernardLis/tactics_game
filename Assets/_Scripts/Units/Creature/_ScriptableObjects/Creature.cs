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
        public const int DeathPenaltyBase = 1; // HERE: testing 30
        public const int DeathPenaltyPerLevel = 1; // 5

        [Header("Creature")] public int UpgradeTier;

        [FormerlySerializedAs("CreatureAbility")] public Ability.Ability Ability;

        public GameObject Projectile;
        public GameObject HitPrefab;

        float _baseCatchingPower;

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

            _baseCatchingPower = globalUpgradeBoard.GetUpgradeByName("Catching Power").GetValue() * 0.01f;

            if (UnitName.Length == 0) UnitName = Helpers.ParseScriptableObjectName(name);
        }

        public bool IsAbilityUnlocked()
        {
            return Level.Value >= Ability.UnlockLevel;
        }

        public float CalculateChanceToCatch(Hero.Hero hero)
        {
            float chance = _baseCatchingPower;
            // missing health %, *0.5f make it "less" important
            float missingHealthPercent = (1 - (float)CurrentHealth.Value / (float)MaxHealth.GetValue()) * 0.5f;
            chance += missingHealthPercent;
            // difference in level between creature and hero
            int levelDifference = hero.Level.Value - Level.Value;
            chance += 0.1f * levelDifference;
            return 1;

            return chance; // HERE: return chance
        }

        public void Caught(Hero.Hero hero)
        {
            Team = 0;
            CurrentHealth.SetValue(MaxHealth.GetValue());
            hero.AddToTroops(this);
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