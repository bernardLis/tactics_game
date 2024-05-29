using UnityEngine;

namespace Lis.Units.Enemy
{
    public class Enemy : Unit
    {
        [Header("Enemy")]
        [HideInInspector] public bool IsMiniBoss;

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
    }
}