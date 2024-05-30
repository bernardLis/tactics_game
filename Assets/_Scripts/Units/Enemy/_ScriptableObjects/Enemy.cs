using Lis.Core;
using UnityEngine;

namespace Lis.Units.Enemy
{
    public class Enemy : Unit
    {
        [HideInInspector] public bool IsMiniBoss;

        [Header("Enemy")]
        public int ScarinessRank;

        public void SetMiniBoss()
        {
            IsMiniBoss = true;
        }

        public override void InitializeBattle(int team)
        {
            base.InitializeBattle(team);

            if (!IsMiniBoss) return;
            // Set mini boss stats
            MaxHealth.BonusValue = MaxHealth.BaseValue;
            CurrentHealth.SetValue(MaxHealth.GetValue());

            Power.BonusValue = Power.BaseValue;
            Armor.BonusValue = Armor.BaseValue;
            Speed.BonusValue = 1;
        }

        public int EnemyMaxHealth;
        public int EnemyArmor;
        public float EnemySpeed;
        public int EnemyPower;

        protected override void CreateStats()
        {
            MaxHealth = CreateInstance<Stat>();
            MaxHealth.StatType = StatType.Health;
            MaxHealth.SetBaseValue(EnemyMaxHealth);
            MaxHealth.Initialize();

            Armor = CreateInstance<Stat>();
            Armor.StatType = StatType.Armor;
            Armor.SetBaseValue(EnemyArmor);
            Armor.Initialize();

            Speed = CreateInstance<Stat>();
            Speed.StatType = StatType.Speed;
            Speed.SetBaseValue(EnemySpeed);
            Speed.Initialize();

            Power = CreateInstance<Stat>();
            Power.StatType = StatType.Power;
            Power.SetBaseValue(EnemyPower);
            Power.Initialize();
        }
    }
}