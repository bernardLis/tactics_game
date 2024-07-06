using System;
using System.Collections.Generic;
using Lis.Core;
using Lis.HeroCreation;
using Lis.Units.Hero.Ability;
using Lis.Units.Hero.Items;
using Lis.Units.Hero.Tablets;
using Lis.Upgrades;
using UnityEngine;

namespace Lis.Units.Hero
{
    [CreateAssetMenu(menuName = "ScriptableObject/Units/Hero/Hero")]
    public class Hero : Unit
    {
        [Header("Visuals")]
        public VisualHero VisualHero;

        [Header("Hero Sounds")]
        public Sound TeleportStartSound;

        public Sound TeleportEndSound;

        [Header("Hero Stats")]
        public Stat Pull;

        public Stat BonusExp;

        [HideInInspector] public List<Unit> Army = new();

        [Header("Tablets")] [HideInInspector] public List<Tablet> Tablets = new();
        [HideInInspector] public TabletAdvanced AdvancedTablet;

        [Header("Abilities")]
        public Ability.Ability StartingAbility;

        public List<Ability.Ability> Abilities = new();
        public List<Ability.Ability> AdvancedAbilities = new();
        readonly Dictionary<NatureName, Tablet> _tabletsByElement = new();
        GameManager _gameManager;

        public void InitializeHero(VisualHero visualHero)
        {
            VisualHero = visualHero;
            UnitName = visualHero.Name;

            _gameManager = GameManager.Instance;

            CreateBaseStats();
            CreateTablets();
            foreach (Tablet t in Tablets)
                t.Initialize(this);

            Abilities = new();
        }

        public void AddArmy(Unit armyUnit)
        {
            Army.Add(armyUnit);
            if (armyUnit is Peasant.Peasant peasant)
                peasant.OnUpgraded += _ => Army.Remove(peasant);
        }

        protected override void CreateStats()
        {
            // Hero handles it through CreateBaseStats() instead
        }

        [Header("Armor")]
        public ArmorSlot[] ArmorSlots;

        public void AddArmor(Armor newArmor)
        {
            ArmorSlot slot = Array.Find(ArmorSlots, s => s.ItemType == newArmor.ItemType);
            slot.PreviousItem = slot.CurrentItem;
            slot.CurrentItem = newArmor;

            VisualHero.SetItem(newArmor);

            RemoveArmor(slot.PreviousItem);
            ApplyArmor(newArmor);
        }

        void ApplyArmor(Armor toApply)
        {
            Stat armorStat = GetStatByType(toApply.StatType);
            armorStat.ApplyBonusValueChange(toApply.Value);
        }

        void RemoveArmor(Armor toRemove)
        {
            if (toRemove is null) return;
            Stat armorStat = GetStatByType(toRemove.StatType);
            armorStat.ApplyBonusValueChange(-toRemove.Value);
        }

        /* LEVELING */
        int GetExpValue(float gain)
        {
            return Mathf.CeilToInt(gain);
        }

        public override void AddExp(float gain)
        {
            base.AddExp(GetExpValue(gain));
        }

        protected override int GetExpForNextLevel()
        {
            // TODO: balance
            const float exponent = 2.5f;
            const float multiplier = 10f;
            const int baseExp = 100;

            int result = Mathf.FloorToInt(multiplier * Mathf.Pow(Level.Value, exponent));
            result = Mathf.RoundToInt(result * 0.1f) * 10; // rounding to tens
            int expRequired = result + baseExp;

            return expRequired;
        }

        public event Action<TabletAdvanced> OnTabletAdvancedAdded;

        void CreateTablets()
        {
            if (Tablets.Count > 0) return; // safety check
            foreach (Tablet original in _gameManager.UnitDatabase.HeroTablets)
            {
                Tablet instance = Instantiate(original);
                Tablets.Add(instance);
                instance.Initialize(this);
                instance.OnLevelUp += CheckAdvancedTablets;
            }
        }

        public Tablet GetTabletByElement(NatureName natureName)
        {
            if (_tabletsByElement.Count < Tablets.Count)
                foreach (Tablet t in Tablets)
                    _tabletsByElement.Add(t.Nature.NatureName, t);

            return _tabletsByElement.GetValueOrDefault(natureName);
        }

        void CheckAdvancedTablets(Tablet tablet)
        {
            if (AdvancedTablet != null) return; // only one advanced tablet
            if (!tablet.IsMaxLevel()) return;

            Nature first = tablet.Nature;
            foreach (Tablet t in Tablets)
                if (t.IsMaxLevel() && t != tablet)
                {
                    TabletAdvanced adv = _gameManager.UnitDatabase.GetAdvancedTabletByNatureNames(first.NatureName,
                        t.Nature.NatureName);
                    if (adv == null) continue;
                    AddAdvancedTablet(adv);
                    return;
                }
        }

        public void AddAdvancedTablet(TabletAdvanced original)
        {
            AdvancedTablet = Instantiate(original);
            AdvancedTablet.Initialize(this);

            _tabletsByElement.Add(AdvancedTablet.Nature.NatureName, AdvancedTablet);

            OnTabletAdvancedAdded?.Invoke(AdvancedTablet);
        }

        public event Action<Ability.Ability> OnAbilityAdded;
        public event Action<Ability.Ability> OnAbilityRemoved;

        public void AddAbility(Ability.Ability ability)
        {
            Ability.Ability instance = Instantiate(ability);
            instance.InitializeBattle(this);

            if (ability.IsAdvanced)
                AdvancedAbilities.Add(instance);
            else
                Abilities.Add(instance);

            OnAbilityAdded?.Invoke(instance);
        }

        public Ability.Ability GetAbilityById(string id)
        {
            foreach (Ability.Ability a in Abilities)
                if (a.Id == id)
                    return a;
            return null;
        }

        public List<Ability.Ability> GetAllAbilities()
        {
            List<Ability.Ability> allAbilities = new();
            allAbilities.AddRange(Abilities);
            allAbilities.AddRange(AdvancedAbilities);
            return allAbilities;
        }

        public void RemoveAbility(Ability.Ability a)
        {
            if (a.IsAdvanced)
                AdvancedAbilities.Remove(a);
            else
                Abilities.Remove(a);
            OnAbilityRemoved?.Invoke(a);
        }

        void CreateBaseStats()
        {
            Level = CreateInstance<IntVariable>();
            Level.SetValue(1);

            MaxHealth = Instantiate(MaxHealth);
            OnLevelUp += MaxHealth.LevelUp;
            MaxHealth.Initialize();

            Armor = Instantiate(Armor);
            Armor.Initialize();

            Speed = Instantiate(Speed);
            Speed.Initialize();

            Pull = Instantiate(Pull);
            Pull.Initialize();

            Power = Instantiate(Power);
            Power.Initialize();

            BonusExp = Instantiate(BonusExp);
            BonusExp.Initialize();

            UpgradeBoard globalUpgradeBoard = _gameManager.UpgradeBoard;
            MaxHealth.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Hero Health").GetValue());
            Armor.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Hero Armor").GetValue());
            Speed.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Hero Speed").GetValue());
            Pull.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Hero Pull").GetValue());
            Power.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Hero Power").GetValue());
            BonusExp.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Hero Exp Bonus").GetValue());
        }

        public List<Stat> GetAllStats()
        {
            List<Stat> stats = new()
            {
                MaxHealth,
                Armor,
                Speed,
                Pull,
                Power,
                BonusExp
            };
            return stats;
        }

        public Stat GetStatByType(StatType type)
        {
            switch (type)
            {
                case StatType.Health:
                    return MaxHealth;
                case StatType.Armor:
                    return Armor;
                case StatType.Speed:
                    return Speed;
                case StatType.Pull:
                    return Pull;
                case StatType.Power:
                    return Power;
                case StatType.ExpBonus:
                    return BonusExp;
                default:
                    return null;
            }
        }

        public int GetHeroPoints()
        {
            int points = Level.Value * 100; // TODO: balance, idk if levels are that valuable
            foreach (Unit u in Army)
            {
                if (u.CurrentHealth.Value <= 0) continue;
                points += u.Price;
            }

            foreach (Ability.Ability a in Abilities)
                points += a.GetCurrentLevel().Price;

            points += GameManager.Instance.Gold;

            // TODO: price hero tablets
            return points;
        }

        /* SERIALIZATION */
        public new HeroData SerializeSelf()
        {
            return new();
        }

        public void LoadFromData(HeroData data)
        {
            _gameManager = GameManager.Instance;
            UnitDatabase heroDatabase = _gameManager.UnitDatabase;

            Id = data.Id;

            CreateBaseStats();

            CreateStats();

            foreach (AbilityData abilityData in data.AbilityData)
            {
                Ability.Ability a = Instantiate(heroDatabase.GetAbilityById(abilityData.TemplateId));
                a.LoadFromData(abilityData);
                Abilities.Add(a);
            }
        }
    }

    [Serializable]
    public struct HeroData
    {
        public string Id;

        public List<AbilityData> AbilityData;
    }

    [Serializable]
    public struct ArmorSlot
    {
        public ItemType ItemType;
        public Armor CurrentItem;
        public Armor PreviousItem;
    }
}