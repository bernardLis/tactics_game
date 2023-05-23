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

    Label _kills;
    Label _damageDealt;
    Label _damageTaken;

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

        _kills = new($"Killed enemies: {_battleEntity.KilledEnemiesCount}");
        _damageDealt = new($"Damage dealt: {_battleEntity.DamageDealt}");
        _damageTaken = new($"Damage taken: {_battleEntity.DamageTaken}");

        _rightContainer.Add(_kills);
        _rightContainer.Add(_damageDealt);
        _rightContainer.Add(_damageTaken);

        _battleEntity.OnHealthChanged += OnHealthChanged;
        _battleEntity.OnEnemyKilled += OnEnemyKilled;
        _battleEntity.OnDamageDealt += OnDamageDealt;
        _battleEntity.OnDamageTaken += OnDamageTaken;
    }

    void OnHealthChanged(float nvm)
    {
        _healthLabel.text = $"Health: {_battleEntity.CurrentHealth} / {_armyEntity.Health}";
    }

    void OnEnemyKilled(int total)
    {
        _kills.text = $"Killed enemies: {total}";
    }

    void OnDamageDealt(int dmg)
    {
        _damageDealt.text = $"Damage dealt: {_battleEntity.DamageDealt}";
    }

    void OnDamageTaken(int dmg)
    {
        _damageTaken.text = $"Damage taken: {_battleEntity.DamageTaken}";
    }
}
