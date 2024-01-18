using System;
using UnityEngine;

namespace Lis
{
    public class EntityMovement : Entity
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
        new public EntityMovementData SerializeSelf()
        {
            // TODO: to be implemented
            EntityMovementData data = new()
            {
                EntityBaseData = base.SerializeSelf(),

                Speed = Speed.BaseValue,
            };

            return data;
        }

        public void LoadFromData(EntityMovementData data)
        {
            // TODO: to be implemented

        }
    }

    [Serializable]
    public struct EntityMovementData
    {
        public EntityData EntityBaseData;

        public int Speed;
    }
}