using System;
using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Attack;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Units
{
    [CreateAssetMenu(menuName = "ScriptableObject/Units/Unit")]

    public class Unit : BaseScriptableObject
    {
        [Header("Unit")] public string UnitName;
        public Sprite Icon;
        public Sprite[] IconAnimation;
        public int Price;
        public Nature Nature;
        public GameObject Prefab;

        [Header("Sounds")]
        public Sound SpawnSound;

        public Sound DeathSound;
        public Sound AttackSound;
        public Sound HitSound;
        public Sound LevelUpSound;

        [HideInInspector] public int TotalKillCount;

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

            InstantiateAttacks();
            ChooseAttack();
        }

        public virtual void AddKill(Unit unit)
        {
            TotalKillCount++;
        }

        [Header("Stats")]
        public Stat MaxHealth;

        [HideInInspector] public FloatVariable CurrentHealth;

        public Stat Armor;
        public Stat Speed;
        public Stat Power;

        protected virtual void CreateStats()
        {
            HandleMaxHealth();
            HandleArmor();
            HandleSpeed();
            HandlePower();
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

        /* ATTACKS */
        [SerializeField] Attack.Attack[] _attacksOriginal;
        public List<Attack.Attack> Attacks = new();
        [HideInInspector] public Attack.Attack CurrentAttack;

        void InstantiateAttacks()
        {
            foreach (Attack.Attack a in _attacksOriginal)
            {
                Attack.Attack attack = Instantiate(a);
                Attacks.Add(attack);
            }
        }

        public void AddAttack(Attack.Attack attack)
        {
            Attacks.Add(attack);
        }

        public AttackController ChooseAttack()
        {
            float lowestCooldown = Mathf.Infinity;
            CurrentAttack = Attacks[Random.Range(0, Attacks.Count)];

            foreach (Attack.Attack a in Attacks)
            {
                if (a.AttackController == null) continue;
                if (!(a.AttackController.CurrentCooldown < lowestCooldown)) continue;
                CurrentAttack = a;
                lowestCooldown = a.AttackController.CurrentCooldown;
            }

            return CurrentAttack.AttackController;
        }

        public int GetDamageDealt()
        {
            int damageDealt = 0;
            foreach (Attack.Attack a in Attacks)
                damageDealt += a.DamageDealt;

            return damageDealt;
        }

        /* DAMAGE */
        public int CalculateDamage(Attack.Attack attack)
        {
            float dmg = attack.GetDamage();
            dmg *= GetElementalDamageMultiplier(attack.Nature);
            if (!attack.IsArmorPiercing) dmg -= Armor.GetValue();
            if (dmg < 0) dmg = 0;
            return Mathf.RoundToInt(dmg);
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