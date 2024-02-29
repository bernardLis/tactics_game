using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Units
{
    public class UnitMovement : Unit
    {
        [Header("Movement")]
        public Stat Speed;

        protected override void CreateStats()
        {
            base.CreateStats();

            Speed = Instantiate(Speed);
            Speed.Initialize();
            OnLevelUp += Speed.LevelUp;
        }

        /* SERIALIZATION */
        new public UnitMovementData SerializeSelf()
        {
            // TODO: to be implemented
            UnitMovementData data = new()
            {
                UnitBaseData = base.SerializeSelf(),

                Speed = Speed.BaseValue,
            };

            return data;
        }

        public void LoadFromData(UnitMovementData data)
        {
            // TODO: to be implemented
        }
    }

    [Serializable]
    public struct UnitMovementData
    {
        public UnitData UnitBaseData;

        public float Speed;
    }
}