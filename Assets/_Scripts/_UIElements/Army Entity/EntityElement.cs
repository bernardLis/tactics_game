using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EntityElement : VisualElement
{
    const string _ussClassName = "entity-element__";
    const string _ussMain = _ussClassName + "main";
    const string _ussLeftContainer = _ussClassName + "left-container";
    const string _ussName = _ussClassName + "name";

    GameManager _gameManager;

    ArmyEntity _entity;

    protected VisualElement _leftContainer;
    protected VisualElement _middleContainer;
    protected VisualElement _rightContainer;

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

        _leftContainer = new();
        _leftContainer.AddToClassList(_ussLeftContainer);
        _middleContainer = new();
        _rightContainer = new();
        Add(_leftContainer);
        Add(_middleContainer);
        Add(_rightContainer);

        _elementalElement = new ElementalElement(_entity.Element);
        _entityIcon = new(_entity);
        _nameLabel = new Label();
        _nameLabel.AddToClassList(_ussName);

        _healthLabel = new Label($"Health:");
        _power = new Label($"Power:");
        _armor = new Label($"Armor:");
        _attackRange = new Label($"Attack Range:");
        _attackCooldown = new Label($"Attack Cooldown:");
        _speed = new Label($"Speed:");

        _leftContainer.Add(_nameLabel);
        _leftContainer.Add(_entityIcon);
        _leftContainer.Add(_elementalElement);

        _middleContainer.Add(_healthLabel);
        _middleContainer.Add(_power);
        _middleContainer.Add(_armor);
        _middleContainer.Add(_attackRange);
        _middleContainer.Add(_attackCooldown);
        _middleContainer.Add(_speed);

        if (_entity.EntityAbility != null)
        {
            _rightContainer.Add(new EntityAbilityElement(_entity.EntityAbility));
            _rightContainer.Add(new EntityAbilityTooltipElement(_entity.EntityAbility));
        }

        SetValues(_entity);
    }

    public void SetValues(ArmyEntity entity)
    {
        _entity = entity;

        _entityIcon.SwapEntity(entity);
        _nameLabel.text = $"{entity.Name}";
        _healthLabel.text = $"Health: {entity.Health}";
        _power.text = $"Power: {entity.Power}";
        _armor.text = $"Armor: {entity.Armor}";
        _attackRange.text = $"Attack Range: {entity.AttackRange}";
        _attackCooldown.text = $"Attack Cooldown: {entity.AttackCooldown}";
        _speed.text = $"Speed: {entity.Speed}";
    }
}
