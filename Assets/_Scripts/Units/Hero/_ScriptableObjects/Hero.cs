using System;
using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero.Ability;
using Lis.Units.Hero.Tablets;
using Lis.Upgrades;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Units.Hero
{
    using Creature;

    [CreateAssetMenu(menuName = "ScriptableObject/Hero/Hero")]
    public class Hero : UnitMovement
    {
        GameManager _gameManager;

        [Header("Selector")]
        public int TimesPicked;

        public GameObject SelectorPrefab;

        [Header("Stats")] public Stat Power;
        public Stat Pull;
        public Stat BonusExp;

        public void InitializeHero()
        {
            _gameManager = GameManager.Instance;

            CreateBaseStats();
            CreateTablets();
            foreach (Tablet t in Tablets)
                t.Initialize(this);

            Abilities = new();

            AddRandomArmy(); // TODO: for now
        }

        [HideInInspector] public List<Creature> Army = new();

        public void AddArmy(Creature armyUnit)
        {
            Army.Add(armyUnit);
        }

        void AddRandomArmy()
        {
            List<Creature> availableCreatures = new(GameManager.Instance.UnitDatabase.AllCreatures);
            for (int i = 0; i < 3; i++)
            {
                Creature instance = Instantiate(availableCreatures[Random.Range(0, availableCreatures.Count)]);
                instance.InitializeBattle(0);
                Army.Add(instance);
            }
        }

        protected override void CreateStats()
        {
            // Hero handles it through CreateBaseStats() instead
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

        public override int GetExpForNextLevel()
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

        [Header("Tablets")] public List<Tablet> Tablets = new();
        public TabletAdvanced AdvancedTablet;
        public event Action<TabletAdvanced> OnTabletAdvancedAdded;
        readonly Dictionary<NatureName, Tablet> _tabletsByElement = new();

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

            return !_tabletsByElement.ContainsKey(natureName) ? null : _tabletsByElement[natureName];
        }

        void CheckAdvancedTablets(Tablet tablet)
        {
            if (AdvancedTablet != null) return; // only one advanced tablet
            if (!tablet.IsMaxLevel()) return;

            Nature first = tablet.Nature;
            foreach (Tablet t in Tablets)
            {
                if (t.IsMaxLevel() && t != tablet)
                {
                    TabletAdvanced adv = _gameManager.UnitDatabase.GetAdvancedTabletByNatureNames(first.NatureName,
                        t.Nature.NatureName);
                    if (adv == null) continue;
                    AddAdvancedTablet(adv);
                    return;
                }
            }
        }

        public void AddAdvancedTablet(TabletAdvanced original)
        {
            AdvancedTablet = Instantiate(original);
            AdvancedTablet.Initialize(this);

            _tabletsByElement.Add(AdvancedTablet.Nature.NatureName, AdvancedTablet);

            OnTabletAdvancedAdded?.Invoke(AdvancedTablet);
        }

        [Header("Abilities")]
        public Ability.Ability StartingAbility;

        public List<Ability.Ability> Abilities = new();
        public List<Ability.Ability> AdvancedAbilities = new();
        public event Action<Ability.Ability> OnAbilityAdded;

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

        public void StopAllAbilities()
        {
            foreach (Ability.Ability a in Abilities)
                a.StopAbility();
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

        void CreateBaseStats()
        {
            Level = CreateInstance<IntVariable>();
            Level.SetValue(1);

            MaxHealth = Instantiate(MaxHealth);
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

        /* SERIALIZATION */
        public new HeroData SerializeSelf()
        {
            HeroData data = new()
            {
                UnitMovementData = base.SerializeSelf(),
            };

            List<AbilityData> abilityData = new();
            foreach (Ability.Ability a in Abilities)
                abilityData.Add(a.SerializeSelf());
            data.AbilityData = abilityData;

            return data;
        }

        public void LoadFromData(HeroData data)
        {
            _gameManager = GameManager.Instance;
            UnitDatabase heroDatabase = _gameManager.UnitDatabase;

            Id = data.Id;

            CreateBaseStats();
            LoadFromData(data.UnitMovementData);

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
        public UnitMovementData UnitMovementData;
        public string Id;

        public List<AbilityData> AbilityData;
    }
}