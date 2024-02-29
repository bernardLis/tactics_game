using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Units
{
    public class UnitFight : UnitMovement
    {
        [Header("Fight")]
        public Stat Power;

        public Stat AttackRange;
        public Stat AttackCooldown;

        [HideInInspector] public int OldKillCount;
        [HideInInspector] public int TotalKillCount;
        [HideInInspector] public int OldDamageDealt;
        [HideInInspector] public int TotalDamageDealt;

        public override void InitializeBattle(int team)
        {
            base.InitializeBattle(team);
            OldKillCount = TotalKillCount;
            OldDamageDealt = TotalDamageDealt;
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

        protected override void CreateStats()
        {
            base.CreateStats();

            Power = Instantiate(Power);
            AttackRange = Instantiate(AttackRange);
            AttackCooldown = Instantiate(AttackCooldown);

            Power.Initialize();
            AttackRange.Initialize();
            AttackCooldown.Initialize();

            OnLevelUp += Power.LevelUp;
            OnLevelUp += AttackRange.LevelUp;
            OnLevelUp += AttackCooldown.LevelUp;
        }

        /* SERIALIZATION */
        new public UnitFightData SerializeSelf()
        {
            // TODO: to be implemented
            UnitFightData data = new()
            {
                UnitMovementData = base.SerializeSelf(),
            };

            return data;
        }

        public void LoadFromData(UnitFightData data)
        {
            // TODO: to be implemented
        }
    }

    [Serializable]
    public struct UnitFightData
    {
        public UnitMovementData UnitMovementData;

        public int Power;
        public int AttackRange;
        public int AttackCooldown;
    }
}