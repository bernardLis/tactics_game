using System;
using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero.Ability;
using Lis.Units.Hero.Tablets;
using Lis.Upgrades;
using UnityEngine;

namespace Lis.Units.Hero
{
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

        bool _isInitialized; //HERE: testing (mostly)

        public void InitializeHero()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _gameManager = GameManager.Instance;

            CreateBaseStats();
            CreateTablets();
            foreach (Tablet t in Tablets)
                t.Initialize(this);

            Abilities = new();
        }

        public override void InitializeBattle(int team)
        {
            base.InitializeBattle(team);
            UpgradeBoard globalUpgradeBoard = GameManager.Instance.UpgradeBoard;

            NumberOfFriendBalls = globalUpgradeBoard.GetUpgradeByName("Starting Friend Balls").GetValue();
            TroopsLimit = CreateInstance<IntVariable>();
            TroopsLimit.SetValue(2 + globalUpgradeBoard.GetUpgradeByName("Starting Troops Limit").GetValue());
        }

        protected override void CreateStats()
        {
            // Hero handles it through CreateBaseStats() instead 
        }

        public int CalculateDamage(Minion.Minion minion)
        {
            float damage = minion.GetPower();

            damage -= Armor.GetValue();
            if (damage < 0) damage = 0;

            // abilities ignore armor
            return Mathf.RoundToInt(damage);
        }

        /* FRIEND BALLS */
        public int NumberOfFriendBalls;
        public event Action OnFriendBallCountChanged;

        public bool HasFriendBalls()
        {
            return NumberOfFriendBalls > 0;
        }

        public void UseFriendBall()
        {
            if (!HasFriendBalls()) return;
            NumberOfFriendBalls--;
            OnFriendBallCountChanged?.Invoke();
        }

        public void AddFriendBalls(int amount)
        {
            NumberOfFriendBalls += amount;
            OnFriendBallCountChanged?.Invoke();
        }

        /* TROOPS */
        public IntVariable TroopsLimit;
        public List<Creature.Creature> Troops = new();

        public event Action<Creature.Creature> OnTroopMemberAdded;

        public bool CanAddToTroops()
        {
            return Troops.Count < TroopsLimit.Value;
        }

        public void AddToTroops(Creature.Creature creature)
        {
            if (!CanAddToTroops()) return;
            Troops.Add(creature);
            OnTroopMemberAdded?.Invoke(creature);
        }

        /* LEVELING */
        int GetExpValue(float gain)
        {
            return Mathf.CeilToInt(gain + gain * BonusExp.GetValue() * 0.01f);
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
        public Dictionary<NatureName, Tablet> TabletsByElement = new();

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
            if (TabletsByElement.Count < Tablets.Count)
                foreach (Tablet t in Tablets)
                    TabletsByElement.Add(t.Nature.NatureName, t);

            return !TabletsByElement.ContainsKey(natureName) ? null : TabletsByElement[natureName];
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

            // // TODO: this can be handled better
            // if (tablet.Nature.NatureName == NatureName.Earth)
            // {
            //     if (GetTabletByElement(NatureName.Fire).IsMaxLevel())
            //         AddAdvancedTablet(NatureName.Earth, NatureName.Fire);
            //     else if (GetTabletByElement(NatureName.Water).IsMaxLevel())
            //         AddAdvancedTablet(NatureName.Earth, NatureName.Water);
            // }
            //
            // if (tablet.Nature.NatureName == NatureName.Fire)
            // {
            //     if (GetTabletByElement(NatureName.Earth).IsMaxLevel())
            //         AddAdvancedTablet(NatureName.Fire, NatureName.Earth);
            //     else if (GetTabletByElement(NatureName.Wind).IsMaxLevel())
            //         AddAdvancedTablet(NatureName.Fire, NatureName.Wind);
            // }
            //
            // if (tablet.Nature.NatureName == NatureName.Water)
            // {
            //     if (GetTabletByElement(NatureName.Wind).IsMaxLevel())
            //         AddAdvancedTablet(NatureName.Water, NatureName.Wind);
            //     else if (GetTabletByElement(NatureName.Earth).IsMaxLevel())
            //         AddAdvancedTablet(NatureName.Water, NatureName.Earth);
            // }
            //
            // if (tablet.Nature.NatureName == NatureName.Wind)
            // {
            //     if (GetTabletByElement(NatureName.Water).IsMaxLevel())
            //         AddAdvancedTablet(NatureName.Wind, NatureName.Water);
            //     else if (GetTabletByElement(NatureName.Fire).IsMaxLevel())
            //         AddAdvancedTablet(NatureName.Wind, NatureName.Fire);
            // }

            // NatureName firstNature = NatureName.None;
            // foreach (Tablet t in Tablets)
            // {
            //     if (!t.IsMaxLevel()) continue;
            //     if (firstNature == NatureName.None)
            //     {
            //         firstNature = t.Nature.NatureName;
            //         continue;
            //     }
            //
            //     NatureName secondNature = t.Nature.NatureName;
            //     AddAdvancedTablet(firstNature, secondNature);
            //     break;
            // }
        }

        void AddAdvancedTablet(TabletAdvanced original)
        {
            AdvancedTablet = Instantiate(original);
            AdvancedTablet.Initialize(this);

            TabletsByElement.Add(AdvancedTablet.Nature.NatureName, AdvancedTablet);

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