using System;
using Lis.Core;
using Lis.Units.Hero.Ability;
using UnityEngine;

namespace Lis.Units
{
    public class Unit : BaseScriptableObject
    {
        [Header("Unit")] public string UnitName;
        public Sprite Icon;
        public Sprite[] IconAnimation;
        public int Price;
        public Nature Nature;
        public GameObject Prefab;

        [HideInInspector] public int TotalDamageTaken;
        [HideInInspector] public int TotalKillCount;
        [HideInInspector] public int TotalDamageDealt;

        [HideInInspector] public int Team;

        public virtual void InitializeBattle(int team)
        {
            Team = team;

            CreateStats();

            Level = Instantiate(Level);

            Experience = CreateInstance<FloatVariable>();
            Experience.SetValue(0);

            ExpForNextLevel = CreateInstance<FloatVariable>();
            ExpForNextLevel.SetValue(GetExpForNextLevel());

            CurrentHealth = CreateInstance<FloatVariable>();
            CurrentHealth.SetValue(MaxHealth.GetValue());
        }

        public void AddDmgTaken(int dmg)
        {
            TotalDamageTaken += dmg;
        }

        public void AddKill(Unit unit)
        {
            AddExp(unit.Price);
            TotalKillCount++;
        }

        public void AddDmgDealt(int dmg)
        {
            TotalDamageDealt += dmg;
        }


        [Header("Stats")]
        public Stat MaxHealth;

        [HideInInspector] public FloatVariable CurrentHealth;

        public Stat Armor;
        public Stat Speed;
        public Stat Power;
        public Stat AttackRange;
        public Stat AttackCooldown;

        protected virtual void CreateStats()
        {
            HandleMaxHealth();
            HandleArmor();
            HandleSpeed();
            HandlePower();
            HandleAttackRange();
            HandleAttackCooldown();
        }

        void HandleMaxHealth()
        {
            if (MaxHealth == null) return;
            MaxHealth = Instantiate(MaxHealth);
            MaxHealth.Initialize();
            OnLevelUp += MaxHealth.LevelUp;
        }

        void HandleArmor()
        {
            if (Armor == null) return;
            Armor = Instantiate(Armor);
            Armor.Initialize();
            OnLevelUp += Armor.LevelUp;
        }

        void HandleSpeed()
        {
            if (Speed == null) return;
            Speed = Instantiate(Speed);
            Speed.Initialize();
            OnLevelUp += Speed.LevelUp;
        }

        void HandlePower()
        {
            if (Power == null) return;
            Power = Instantiate(Power);
            Power.Initialize();
            OnLevelUp += Power.LevelUp;
        }

        void HandleAttackRange()
        {
            if (AttackRange == null) return;
            AttackRange = Instantiate(AttackRange);
            AttackRange.Initialize();
            OnLevelUp += AttackRange.LevelUp;
        }

        void HandleAttackCooldown()
        {
            if (AttackCooldown == null) return;
            AttackCooldown = Instantiate(AttackCooldown);
            AttackCooldown.Initialize();
            OnLevelUp += AttackCooldown.LevelUp;
        }

        /* LEVEL */
        [Header("Level")] public IntVariable Level;
        [HideInInspector] public FloatVariable Experience;
        [HideInInspector] public FloatVariable ExpForNextLevel;
        [HideInInspector] public float LeftoverExp;
        public event Action OnLevelUp;

        protected virtual int GetExpForNextLevel()
        {
            // meant to be overwritten
            return 100;
        }

        public virtual void AddExp(float gain)
        {
            LeftoverExp = gain;
            if (Experience.Value + gain >= ExpForNextLevel.Value)
            {
                LeftoverExp = Mathf.FloorToInt(Experience.Value + gain - ExpForNextLevel.Value);
                LevelUp();
            }

            Experience.ApplyChange(LeftoverExp);
            LeftoverExp = 0;
        }

        public void LevelUp()
        {
            Level.ApplyChange(1);
            Experience.SetValue(0);
            ExpForNextLevel.SetValue(GetExpForNextLevel());
            OnLevelUp?.Invoke();

            CurrentHealth.SetValue(MaxHealth.GetValue());
        }

        /* DAMAGE */
        public int CalculateDamage(Unit attacker)
        {
            float damage = attacker.Power.GetValue();
            damage *= GetElementalDamageMultiplier(attacker.Nature);
            damage -= Armor.GetValue();
            if (damage < 0) damage = 0;

            return Mathf.RoundToInt(damage);
        }

        public int CalculateDamage(Ability ability)
        {
            float damage = ability.GetPower();
            damage *= GetElementalDamageMultiplier(ability.Nature);
            if (!ability.IsArmorPiercing) damage -= Armor.GetValue();
            if (damage < 0) damage = 0;

            return Mathf.RoundToInt(damage);
        }


        float GetElementalDamageMultiplier(Nature attackerNature)
        {
            float elementalDamageBonus = 1f;
            if (Nature.StrongAgainst == attackerNature)
                elementalDamageBonus = 0.5f;
            if (Nature.WeakAgainst == attackerNature)
                elementalDamageBonus = 1.5f;

            return elementalDamageBonus;
        }

        /* SERIALIZATION */
        protected UnitData SerializeSelf()
        {
            // TODO: to be implemented

            return new UnitData();
        }

        public void LoadFromData(UnitData data)
        {
        }
    }

    [Serializable]
    public struct UnitData
    {
        public string Name;

        public int Health;
        public int Armor;

        public int Level;
        public int Experience;
    }
}