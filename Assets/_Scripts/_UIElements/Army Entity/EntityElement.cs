using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EntityElement : VisualElement
{
    const string _ussClassName = "entity-element__";
    const string _ussMain = _ussClassName + "main";

    GameManager _gameManager;

    ArmyEntity _entity;

    protected ElementalElement _elementalElement;
    protected EntityIcon _entityIcon;
    protected Label _nameLabel;
    protected Label _healthLabel;
    protected Label _power;
    protected Label _armor;
    protected Label _attackRange;
    protected Label _attackCooldown;
    protected Label _speed;

    public EntityElement(ArmyEntity entity)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.EntityElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _entity = entity;

        AddToClassList(_ussMain);

        _elementalElement = new ElementalElement(_entity.Element);
        _entityIcon = new(_entity);

        _nameLabel = new Label($"Name: {_entity.Name}");
        _healthLabel = new Label($"Health: {_entity.Health}");
        _power = new Label($"Power: {_entity.Power}");
        _armor = new Label($"Armor: {_entity.Armor}");
        _attackRange = new Label($"Attack Range: {_entity.AttackRange}");
        _attackCooldown = new Label($"Attack Cooldown: {_entity.AttackCooldown}");
        _speed = new Label($"Speed: {_entity.Speed}");

        Add(_elementalElement);
        Add(_entityIcon);
        Add(_nameLabel);
        Add(_healthLabel);
        Add(_power);
        Add(_armor);
        Add(_attackRange);
        Add(_attackCooldown);
        Add(_speed);
    }
}
