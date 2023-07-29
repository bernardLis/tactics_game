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

    protected override void HandleHealthBar()
    {
        IntVariable totalHealth = ScriptableObject.CreateInstance<IntVariable>();
        totalHealth.SetValue(Entity.BaseHealth);

        Color c = _gameManager.GameDatabase.GetColorByName("Health").Color;
        _healthBar = new(c, "health", _battleEntity.CurrentHealth, totalHealth);
        _middleContainer.Add(_healthBar);
    }
}
