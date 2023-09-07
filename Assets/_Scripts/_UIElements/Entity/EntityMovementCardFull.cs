using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovementCardFull : EntityBaseCardFull
{

    EntityMovement _entityMovement;
    public EntityMovementCardFull(EntityMovement entity) : base(entity)
    {
        _entityMovement = entity;
    }

    protected override void AddStats()
    {
        base.AddStats();
        
        StatElement speed = new(_entityMovement.Speed);
        _statsContainer.Add(speed);
    }
}
