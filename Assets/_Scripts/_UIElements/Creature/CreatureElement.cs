using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class CreatureElement : VisualElement
{
    const string _ussClassName = "creature-element__";
    const string _ussMain = _ussClassName + "main";
    const string _ussLeftContainer = _ussClassName + "left-container";
    const string _ussName = _ussClassName + "name";

    GameManager _gameManager;

    Creature _creature;

    protected VisualElement _leftContainer;
    protected VisualElement _middleContainer;
    protected VisualElement _rightContainer;

    protected ElementalElement _elementalElement;
    public CreatureIcon CreatureIcon;
    protected Label _nameLabel;
    protected Label _levelLabel;
    protected Label _healthLabel;
    protected Label _power;
    protected Label _armor;
    protected Label _attackCooldown;
    protected Label _speed;

    public CreatureElement(Creature creature)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CreatureElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _creature = creature;

        AddToClassList(_ussMain);

        _leftContainer = new();
        _leftContainer.AddToClassList(_ussLeftContainer);
        _middleContainer = new();
        _rightContainer = new();
        Add(_leftContainer);
        Add(_middleContainer);
        Add(_rightContainer);

        _elementalElement = new ElementalElement(_creature.Element);
        CreatureIcon = new(_creature, true);
        _nameLabel = new Label();
        _nameLabel.AddToClassList(_ussName);

        _levelLabel = new Label($"Level: {_creature.Level}");
        creature.OnLevelUp += () => _levelLabel.text = $"Level: {creature.Level}";

        _healthLabel = new Label($"Health:");
        _power = new Label($"Power:");
        _armor = new Label($"Armor:");
        _attackCooldown = new Label($"Attack Cooldown:");
        _speed = new Label($"Speed:");

        _leftContainer.Add(_nameLabel);
        _leftContainer.Add(CreatureIcon);
        _leftContainer.Add(_elementalElement);

        _middleContainer.Add(_levelLabel);
        _middleContainer.Add(_healthLabel);
        _middleContainer.Add(_power);
        _middleContainer.Add(_armor);
        _middleContainer.Add(_attackCooldown);
        _middleContainer.Add(_speed);

        if (_creature.CreatureAbility != null)
        {
            _rightContainer.Add(new CreatureAbilityElement(_creature.CreatureAbility));
            _rightContainer.Add(new CreatureAbilityTooltipElement(_creature.CreatureAbility));
        }

        SetValues(_creature);
    }

    public void SetValues(Creature creature)
    {
        _creature = creature;

        CreatureIcon.SwapCreature(creature);
        _nameLabel.text = $"{creature.Name}";
        _healthLabel.text = $"Health: {creature.GetHealth()}";
        _power.text = $"Power: {creature.GetPower()}";
        _armor.text = $"Armor: {creature.Armor}";
        _attackCooldown.text = $"Attack Cooldown: {creature.AttackCooldown}";
        _speed.text = $"Speed: {creature.Speed}";
    }
}
