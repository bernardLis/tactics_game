using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleEntityCard : EntityCard
{
    BattleEntity _battleEntity;
    public BattleEntityCard(BattleEntity entity) : base(entity.Entity)
    {
        _battleEntity = entity;

        PopulateCard();
    }
}
