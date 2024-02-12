using System;
using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Creature")]
    public class Creature : EntityFight
    {
        [Header("Creature")] public int UpgradeTier;

        public CreatureAbility CreatureAbility;

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

            if (EntityName.Length == 0) EntityName = Helpers.ParseScriptableObjectName(name);
        }

        public bool IsAbilityUnlocked()
        {
            return Level.Value >= CreatureAbility.UnlockLevel;
        }

        public bool CanUseAbility()
        {
            if (CreatureAbility == null) return false;
            return IsAbilityUnlocked();
        }

        public float CalculateChanceToCatch(Hero hero)
        {
            float chance = _baseCatchingPower;
            Debug.Log("Base chance: " + chance);
            // missing health %, *0.5f make it "less" important
            float missingHealthPercent = (1 - (float)CurrentHealth.Value / (float)MaxHealth.GetValue()) * 0.5f;
            chance += missingHealthPercent;
            // difference in level between creature and hero
            int levelDifference = hero.Level.Value - Level.Value;
            chance += 0.1f * levelDifference;
            return chance;
        }

        public void Caught(Hero hero)
        {
            CurrentHealth.SetValue(MaxHealth.GetValue());
            hero.AddToTroops(this);
            Team = 0;
        }

        new public CreatureData SerializeSelf()
        {
            // TODO: needs to be implemented
            CreatureData data = new()
            {
                CreatureId = Id,

                Name = EntityName,
                Level = Level.Value,

                KillCount = TotalKillCount,
                DamageDealt = TotalDamageDealt,
                DamageTaken = TotalDamageTaken
            };

            return data;
        }

        public void LoadFromData(CreatureData data)
        {
            EntityName = data.Name;

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