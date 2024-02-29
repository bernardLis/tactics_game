using System;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Units.Minion
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Minion")]
    public class Minion : UnitMovement
    {
        [Header("Minion")] public Stat Power;

        public event Action OnDeath;

        public override void InitializeBattle(int team)
        {
            base.InitializeBattle(team);
            if (EntityName.Length == 0) EntityName = Helpers.ParseScriptableObjectName(name);

            if (Level.Value <= 1) return;

            for (int i = 1; i < Level.Value; i++)
            {
                MaxHealth.LevelUp();
                Armor.LevelUp();
                Speed.LevelUp();
                Power.LevelUp();
            }
            
            CurrentHealth.SetValue(MaxHealth.GetValue());
        }


        protected override void CreateStats()
        {
            base.CreateStats();

            Power = Instantiate(Power);
            Power.Initialize();
            OnLevelUp += Power.LevelUp;
        }

        public float GetPower()
        {
            return Power.GetValue();
        }

        public void Death()
        {
            OnDeath?.Invoke();
        }
    }
}