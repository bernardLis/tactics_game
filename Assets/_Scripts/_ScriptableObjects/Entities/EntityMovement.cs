using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement : EntityBase
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
    public EntityBaseData EntityBaseData;

    public int Speed;
}
