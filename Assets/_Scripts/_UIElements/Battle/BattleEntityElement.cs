using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleEntityElement : EntityElement
{
    const string _ussClassName = "battle-entity__";
    const string _ussMain = _ussClassName + "main";

    BattleEntity _battleEntity;
    ArmyEntity _armyEntity;

    Label _killedEnemiesCount;

    public BattleEntityElement(BattleEntity battleEntity) : base(battleEntity.ArmyEntity)
    {
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleEntityElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _battleEntity = battleEntity;
        _armyEntity = _battleEntity.ArmyEntity;

        AddToClassList(_ussMain);

        if (_battleEntity.ArmyEntity.Hero != null)
        {
            _power.text += " + " + Mathf.RoundToInt(_battleEntity.ArmyEntity.Hero.Power.GetValue());
            _armor.text += " + " + Mathf.RoundToInt(_battleEntity.ArmyEntity.Hero.Armor.GetValue());
        }

        _healthLabel.text = $"Health: {_battleEntity.CurrentHealth} / {_armyEntity.Health}";

        _killedEnemiesCount = new($"Killed enemies: {_battleEntity.KilledEnemiesCount}");
        Add(_killedEnemiesCount);

        _battleEntity.OnHealthChanged += OnHealthChanged;
        _battleEntity.OnEnemyKilled += OnEnemyKilled;
    }

    void OnEnemyKilled(int total)
    {
        _killedEnemiesCount.text = $"Killed enemies: {total}";
    }

    void OnHealthChanged(float nvm)
    {
        _healthLabel.text = $"Health: {_battleEntity.CurrentHealth} / {_armyEntity.Health}";
    }


}
