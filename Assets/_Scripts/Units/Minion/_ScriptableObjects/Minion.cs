using System;
using System.Collections.Generic;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Units.Minion
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Minion")]
    public class Minion : UnitMovement
    {
        [Header("Minion")] public Stat Power;
        public List<Nature> AvailableNatures = new();
        public Vector2Int LevelRange;
        public bool IsMiniBoss;

        public event Action OnDeath;

        public override void InitializeBattle(int team)
        {
            base.InitializeBattle(team);
            if (UnitName.Length == 0) UnitName = Helpers.ParseScriptableObjectName(name);

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

        public void SetMiniBoss()
        {
            IsMiniBoss = true;
            MaxHealth.BaseValue *= 20;
            CurrentHealth.SetValue(MaxHealth.GetValue());
            Speed.SetBonusValue(0.5f);
        }

        public void SetRandomNature()
        {
            Nature = AvailableNatures[Random.Range(0, AvailableNatures.Count)];
        }

        public void SetNature(NatureName natureName)
        {
            foreach (Nature n in AvailableNatures)
                if (n.NatureName == natureName)
                    Nature = n;
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