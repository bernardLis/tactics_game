using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class CreatureEvolutionElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary-black";
    const string _ussCommonHorizontalSpacer = "common__horizontal-spacer";

    const string _ussClassName = "army-evolution__";
    const string _ussMain = _ussClassName + "main";
    const string _ussName = _ussClassName + "name";
    const string _ussAbilityContainer = _ussClassName + "ability-container";

    GameManager _gameManager;

    public ArmyGroup ArmyGroup;

    Label _name;
    ArmyGroupElement _armyGroupElement;

    ElementalElement _elementalElement;

    Label _healthLabel;
    Label _power;
    Label _armor;
    Label _attackRange;
    Label _attackCooldown;
    Label _speed;

    VisualElement _abilityContainer;

    ChangingValueElement _killsThisBattleElement;

    int _availableKills;
    IntVariable _kills;
    IntVariable _killsToEvolve;

    public CreatureEvolutionElement(ArmyGroup armyGroup)
    {
        _gameManager = GameManager.Instance;
        var common = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (common != null)
            styleSheets.Add(common);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ArmyEvolutionElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        ArmyGroup = armyGroup;

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        _name = new(armyGroup.Creature.Name);
        _name.AddToClassList(_ussName);
        Add(_name);

        _armyGroupElement = new(armyGroup);
        _armyGroupElement.LargeIcon();
        Add(_armyGroupElement);

        _elementalElement = new ElementalElement(ArmyGroup.Creature.Element);
        Add(_elementalElement);

        _healthLabel = new Label($"Health: {armyGroup.Creature.Health}");
        _power = new Label($"Power: {armyGroup.Creature.Power}");
        _armor = new Label($"Armor: {armyGroup.Creature.Armor}");
        _attackRange = new Label($"Attack Range: {armyGroup.Creature.AttackRange}");
        _attackCooldown = new Label($"Attack Cooldown: {armyGroup.Creature.AttackCooldown}");
        _speed = new Label($"Speed: {armyGroup.Creature.Speed}");

        Add(_healthLabel);
        Add(_power);
        Add(_armor);
        Add(_attackRange);
        Add(_attackCooldown);
        Add(_speed);

        _abilityContainer = new();
        _abilityContainer.AddToClassList(_ussAbilityContainer);
        Add(_abilityContainer);
        _abilityContainer.Add(new CreatureAbilityElement(armyGroup.Creature.CreatureAbility));

        VisualElement spacer = new();
        spacer.AddToClassList(_ussCommonHorizontalSpacer);
        spacer.style.height = 50;
        spacer.style.backgroundImage = null;
        Add(spacer);

        // HERE: build 0.0.2-june2023
        Label warningText = new("Hey! There are upgrades, but they don't have special abilities and are not balanced!");
        warningText.style.color = Color.red;
        warningText.style.whiteSpace = WhiteSpace.Normal;
        warningText.style.maxWidth = 300;
        Add(warningText);

        _availableKills = ArmyGroup.TotalKillCount - ArmyGroup.OldKillCount;
        _kills = ScriptableObject.CreateInstance<IntVariable>();
        _killsToEvolve = ScriptableObject.CreateInstance<IntVariable>();
        _kills.SetValue(ArmyGroup.OldKillCount);
        _killsToEvolve.SetValue(ArmyGroup.NumberOfKillsToEvolve());


    }

    public void ShowKillsThisBattle()
    {
        VisualElement killCountContainer = new();
        killCountContainer.style.flexDirection = FlexDirection.Row;
        killCountContainer.style.alignItems = Align.Center;

        Add(killCountContainer);

        killCountContainer.Add(new Label("Kills this battle:"));
        _killsThisBattleElement = new();
        _killsThisBattleElement.Initialize(_availableKills, 24);

        killCountContainer.Add(_killsThisBattleElement);
        killCountContainer.style.opacity = 0;
        DOTween.To(x => killCountContainer.style.opacity = x, 0, 1, 0.5f);
    }

    public void ShowKillsToEvolveBar()
    {
        Add(new Label("Kills to evolve:"));

        Color barColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        ResourceBarElement killBar = new(barColor, "Kills", _kills, _killsToEvolve);
        killBar.style.width = 200;

        Add(killBar);
        killBar.style.opacity = 0;
        DOTween.To(x => killBar.style.opacity = x, 0, 1, 0.5f);
    }

    public void AddKills()
    {
        int killChange = Mathf.Clamp(_availableKills, 0, ArmyGroup.NumberOfKillsToEvolve());
        _availableKills -= killChange;
        _killsThisBattleElement.ChangeAmount(_availableKills);
        _kills.ApplyChange(killChange);
    }

    public void Evolve(Creature entity)
    {
        _name.text += $" -> {entity.Name}";

        _armyGroupElement.Evolve(entity);

        _healthLabel.text += $" -> {ArmyGroup.Creature.Health}";
        _power.text += $" -> {ArmyGroup.Creature.Power}";
        _armor.text += $" -> {ArmyGroup.Creature.Armor}";
        _attackRange.text += $" -> {ArmyGroup.Creature.AttackRange}";
        _attackCooldown.text += $" -> {ArmyGroup.Creature.AttackCooldown}";
        _speed.text += $" -> {ArmyGroup.Creature.Speed}";

        _abilityContainer.Add(new Label(" -> "));
        if (entity.CreatureAbility != null)
            _abilityContainer.Add(new CreatureAbilityElement(entity.CreatureAbility));

    }

    public void ResetKillsToEvolveBar()
    {
        _killsToEvolve.SetValue(ArmyGroup.NumberOfKillsToEvolve());
    }
}
