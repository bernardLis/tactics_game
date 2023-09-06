using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleEntityCard : EntityCard
{
    BattleEntity _battleEntity;
    public BattleEntityCard(BattleEntity entity) : base(entity.EntityBase)
    {
        _battleEntity = entity;

        PopulateCard();
    }

    protected override void HandleHealthBar()
    {
        Color c = _gameManager.GameDatabase.GetColorByName("Health").Color;
        _healthBar = new(c, "health", _battleEntity.CurrentHealth, _battleEntity.EntityBase.BaseTotalHealth);
        _middleContainer.Add(_healthBar);
    }
}
