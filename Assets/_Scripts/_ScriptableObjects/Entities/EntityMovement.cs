using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement : EntityBase
{
    [Header("Movement")]
    public IntVariable BaseSpeed;
    public Stat Speed { get; protected set; }
    public Vector2Int SpeedGrowthPerLevel;

    protected override void CreateStats()
    {
        base.CreateStats();

        BaseSpeed = Instantiate(BaseSpeed);
        Speed = CreateInstance<Stat>();
        Speed.StatType = StatType.Speed;
        Speed.SetBaseValue(BaseSpeed.Value);
        BaseSpeed.OnValueChanged += Speed.SetBaseValue;
    }

    /* SERIALIZATION */
    new public EntityMovementData SerializeSelf()
    {
        // TODO: to be implemented
        EntityMovementData data = new()
        {
            EntityBaseData = base.SerializeSelf(),

            Speed = BaseSpeed.Value,
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
    public EntityBaseData EntityBaseData;

    public int Speed;
}
