using System;
using Lis.Core;
using Lis.Units.Hero.Ability;
using UnityEngine;
using Element = Lis.Core.Element;

namespace Lis.Units
{
    public class Unit : BaseScriptableObject
    {
        [Header("Unit")] public string EntityName;
        public Sprite Icon;
        public Sprite[] IconAnimation;
        public int Price;
        public Element Element;
        public GameObject Prefab;

        [HideInInspector] public int OldDamageTaken;
        [HideInInspector] public int TotalDamageTaken;

        [HideInInspector] public int Team;

        public virtual void InitializeBattle(int team)
        {
            Team = team;

            CreateStats();
            OldDamageTaken = TotalDamageTaken;

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

        [Header("Stats")] public Stat Armor;
        public Stat MaxHealth;
        [HideInInspector] public FloatVariable CurrentHealth;

        protected virtual void CreateStats()
        {
            MaxHealth = Instantiate(MaxHealth);
            Armor = Instantiate(Armor);

            MaxHealth.Initialize();
            Armor.Initialize();

            OnLevelUp += MaxHealth.LevelUp;
            OnLevelUp += Armor.LevelUp;
        }

        /* LEVEL */
        [Header("Level")] public IntVariable Level;
        [HideInInspector] public FloatVariable Experience;
        [HideInInspector] public FloatVariable ExpForNextLevel;
        [HideInInspector] public float LeftoverExp;
        public event Action OnLevelUp;

        public virtual int GetExpForNextLevel()
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

        public virtual void LevelUp()
        {
            Level.ApplyChange(1);
            Experience.SetValue(0);
            ExpForNextLevel.SetValue(GetExpForNextLevel());
            OnLevelUp?.Invoke();

            CurrentHealth.SetValue(MaxHealth.GetValue());

            // HERE: unit rework - probably need to scale stats with level - change base values
            // but that would be different for creature and for hero
        }

        /* DAMAGE */
        public virtual int CalculateDamage(UnitFight attacker)
        {
            float damage = attacker.Power.GetValue();

            damage *= GetElementalDamageMultiplier(attacker.Element);

            damage -= Armor.GetValue();
            if (damage < 0)
                damage = 0;

            return Mathf.RoundToInt(damage);
        }

        public int CalculateDamage(Ability ability)
        {
            float damage = ability.GetPower();

            damage *= GetElementalDamageMultiplier(ability.Element);

            // abilities ignore armor
            return Mathf.RoundToInt(damage);
        }


        float GetElementalDamageMultiplier(Element attackerElement)
        {
            float elementalDamageBonus = 1f;
            if (Element.StrongAgainst == attackerElement)
                elementalDamageBonus = 0.5f;
            if (Element.WeakAgainst == attackerElement)
                elementalDamageBonus = 1.5f;

            return elementalDamageBonus;
        }

        /* SERIALIZATION */
        public UnitData SerializeSelf()
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