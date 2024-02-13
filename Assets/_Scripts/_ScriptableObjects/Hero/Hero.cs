using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Hero/Player")]
    public class Hero : EntityMovement
    {
        GameManager _gameManager;

        [Header("Stats")] public Stat Power;
        public Stat Pull;
        public Stat BonusExp;

        public override void InitializeBattle(int team)
        {
            base.InitializeBattle(team);
            UpgradeBoard globalUpgradeBoard = GameManager.Instance.UpgradeBoard;

            NumberOfFriendBalls = 2 + globalUpgradeBoard.GetUpgradeByName("Starting Friend Balls").GetValue();
            TroopsLimit = CreateInstance<IntVariable>();
            TroopsLimit.SetValue(2 + globalUpgradeBoard.GetUpgradeByName("Troops Limit").GetValue());
        }

        protected override void CreateStats()
        {
            // Hero handles it through CreateBaseStats() instead 
        }

        public int CalculateDamage(Minion minion)
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
        public List<Creature> Troops = new();

        public event Action<Creature> OnTroopMemberAdded;

        public bool CanAddToTroops()
        {
            return Troops.Count < TroopsLimit.Value;
        }

        public void AddToTroops(Creature creature)
        {
            if (!CanAddToTroops()) return;
            Troops.Add(creature);
            OnTroopMemberAdded?.Invoke(creature);
        }

        /* LEVELING */
        int GetExpValue(int gain)
        {
            return Mathf.CeilToInt(gain + gain * BonusExp.GetValue() * 0.01f);
        }

        public override void AddExp(int gain)
        {
            base.AddExp(GetExpValue(gain));
        }

        public override int GetExpForNextLevel()
        {
            // TODO: math
            const float exponent = 2.5f;
            const float multiplier = 0.7f;
            const int baseExp = 100;

            int result = Mathf.FloorToInt(multiplier * Mathf.Pow(Level.Value, exponent));
            result = Mathf.RoundToInt(result * 0.1f) * 10; // rounding to tens
            int expRequired = result + baseExp;

            return expRequired;
        }

        [Header("Tablets")] public List<Tablet> Tablets = new();
        public TabletAdvanced AdvancedTablet;
        public event Action<TabletAdvanced> OnTabletAdvancedAdded;
        public Dictionary<Element, Tablet> TabletsByElement = new();

        public void CreateTablets()
        {
            foreach (Tablet original in _gameManager.EntityDatabase.HeroTablets)
            {
                Tablet instance = Instantiate(original);
                Tablets.Add(instance);
                instance.Initialize(this);
                instance.OnLevelUp += CheckAdvancedTablets;
            }
        }

        public Tablet GetTabletByElement(Element element)
        {
            if (TabletsByElement.Count < Tablets.Count)
                foreach (Tablet t in Tablets)
                    TabletsByElement.Add(t.Element, t);

            return !TabletsByElement.ContainsKey(element) ? null : TabletsByElement[element];
        }

        void CheckAdvancedTablets()
        {
            if (AdvancedTablet != null) return; // only one advanced tablet

            ElementName firstElement = ElementName.None;
            foreach (Tablet t in Tablets)
            {
                if (!t.IsMaxLevel()) continue;
                if (firstElement == ElementName.None)
                {
                    firstElement = t.Element.ElementName;
                    continue;
                }

                ElementName secondElement = t.Element.ElementName;
                AddAdvancedTablet(firstElement, secondElement);
                break;
            }
        }

        void AddAdvancedTablet(ElementName firstElement, ElementName secondElement)
        {
            TabletAdvanced original =
                _gameManager.EntityDatabase.GetAdvancedTabletByElementNames(firstElement, secondElement);
            if (original == null) return;
            AdvancedTablet = Instantiate(original);
            AdvancedTablet.Initialize(this);

            TabletsByElement.Add(AdvancedTablet.Element, AdvancedTablet);

            OnTabletAdvancedAdded?.Invoke(AdvancedTablet);
        }

        [Header("Abilities")] public List<Ability> Abilities = new();
        public List<Ability> AdvancedAbilities = new();
        public event Action<Ability> OnAbilityAdded;

        public void AddAbility(Ability ability)
        {
            Ability instance = Instantiate(ability);
            instance.InitializeBattle(this);
            Abilities.Add(instance);
            OnAbilityAdded?.Invoke(instance);
        }

        public void StopAllAbilities()
        {
            foreach (Ability a in Abilities)
                a.StopAbility();
        }

        public Ability GetAbilityById(string id)
        {
            foreach (Ability a in Abilities)
                if (a.Id == id)
                    return a;
            return null;
        }

        public void CreateHero(string heroName, Element element)
        {
            _gameManager = GameManager.Instance;

            Id = Guid.NewGuid().ToString();
            name = heroName;

            EntityName = heroName;
            Element = element;

            CreateBaseStats();
            CreateTablets();
            foreach (Tablet t in Tablets)
                t.Initialize(this);

            Abilities = new();
        }

        void CreateBaseStats()
        {
            Level = CreateInstance<IntVariable>();
            Level.SetValue(1);

            EntityDatabase entityDatabase = _gameManager.EntityDatabase;

            MaxHealth = Instantiate(entityDatabase.GetHeroStatByType(StatType.Health));
            MaxHealth.Initialize();

            Armor = Instantiate(entityDatabase.GetHeroStatByType(StatType.Armor));
            Armor.Initialize();

            Speed = Instantiate(entityDatabase.GetHeroStatByType(StatType.Speed));
            Speed.Initialize();

            Pull = Instantiate(entityDatabase.GetHeroStatByType(StatType.Pull));
            Pull.Initialize();

            Power = Instantiate(entityDatabase.GetHeroStatByType(StatType.Power));
            Power.Initialize();

            BonusExp = Instantiate(entityDatabase.GetHeroStatByType(StatType.ExpBonus));
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
                EntityMovementData = base.SerializeSelf(),
            };

            List<AbilityData> abilityData = new();
            foreach (Ability a in Abilities)
                abilityData.Add(a.SerializeSelf());
            data.AbilityData = abilityData;

            return data;
        }

        public void LoadFromData(HeroData data)
        {
            _gameManager = GameManager.Instance;
            EntityDatabase heroDatabase = _gameManager.EntityDatabase;

            Id = data.Id;

            CreateBaseStats();
            LoadFromData(data.EntityMovementData);

            CreateStats();

            foreach (AbilityData abilityData in data.AbilityData)
            {
                Ability a = Instantiate(heroDatabase.GetAbilityById(abilityData.TemplateId));
                a.LoadFromData(abilityData);
                Abilities.Add(a);
            }
        }
    }

    [Serializable]
    public struct HeroData
    {
        public EntityMovementData EntityMovementData;
        public string Id;

        public List<AbilityData> AbilityData;
    }
}