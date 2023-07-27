using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class CreatureCardEvolution : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary-black";
    const string _ussCommonHorizontalSpacer = "common__horizontal-spacer";

    const string _ussClassName = "creature-card-evolution__";
    const string _ussMain = _ussClassName + "main";
    const string _ussName = _ussClassName + "name";
    const string _ussAbilityContainer = _ussClassName + "ability-container";

    GameManager _gameManager;

    public Creature Creature;

    Label _name;
    EntityIcon _creatureIcon;

    ElementalElement _elementalElement;

    Label _healthLabel;
    Label _power;
    Label _armor;
    Label _attackRange;
    Label _attackCooldown;
    Label _speed;

    VisualElement _elementContainer;
    VisualElement _abilityContainer;

    public CreatureCardEvolution(Creature creature)
    {
        _gameManager = GameManager.Instance;
        var common = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (common != null)
            styleSheets.Add(common);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CreatureCardEvolutionStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Creature = creature;

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        _name = new(creature.Name);
        _name.AddToClassList(_ussName);
        Add(_name);

        _creatureIcon = new(creature, true);
        _creatureIcon.LargeIcon();
        Add(_creatureIcon);

        _elementContainer = new();
        _elementContainer.AddToClassList(_ussAbilityContainer);
        Add(_elementContainer);
        _elementalElement = new ElementalElement(Creature.Element);
        _elementContainer.Add(_elementalElement);

        _healthLabel = new Label($"Health: {creature.GetMaxHealth()}");
        _power = new Label($"Power: {creature.GetPower()}");
        _armor = new Label($"Armor: {creature.Armor}");
        _attackRange = new Label($"Attack Range: {creature.AttackRange}");
        _attackCooldown = new Label($"Attack Cooldown: {creature.AttackCooldown}");
        _speed = new Label($"Speed: {creature.Speed}");

        Add(_healthLabel);
        Add(_power);
        Add(_armor);
        Add(_attackRange);
        Add(_attackCooldown);
        Add(_speed);

        _abilityContainer = new();
        _abilityContainer.AddToClassList(_ussAbilityContainer);
        Add(_abilityContainer);
        _abilityContainer.Add(new CreatureAbilityElement(creature.CreatureAbility));

        VisualElement spacer = new();
        spacer.AddToClassList(_ussCommonHorizontalSpacer);
        spacer.style.height = 50;
        spacer.style.backgroundImage = null;
        Add(spacer);
    }

    public void Evolve(Creature creature)
    {
        _name.text += $" -> {creature.Name}";

        _creatureIcon.SwapCreature(creature);

        _elementContainer.Add(new Label(" -> "));
        _elementContainer.Add(new ElementalElement(creature.Element));

        _healthLabel.text += $" -> {creature.GetMaxHealth()}";
        _power.text += $" -> {creature.GetPower()}";
        _armor.text += $" -> {creature.Armor}";
        _attackRange.text += $" -> {creature.AttackRange}";
        _attackCooldown.text += $" -> {creature.AttackCooldown}";
        _speed.text += $" -> {creature.Speed}";

        _abilityContainer.Add(new Label(" -> "));
        if (creature.CreatureAbility != null)
            _abilityContainer.Add(new CreatureAbilityElement(creature.CreatureAbility));
    }
}
