using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class ArmyEvolutionElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary-black";
    const string _ussCommonHorizontalSpacer = "common__horizontal-spacer";

    const string _ussClassName = "army-evolution__";
    const string _ussMain = _ussClassName + "main";

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

    ChangingValueElement _killsThisBattleElement;

    int _availableKills;
    IntVariable _kills;
    IntVariable _killsToEvolve;

    public ArmyEvolutionElement(ArmyGroup armyGroup)
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

        _name = new(armyGroup.ArmyEntity.Name);
        _name.style.fontSize = 32;
        Add(_name);

        _armyGroupElement = new(armyGroup);
        _armyGroupElement.LargeIcon();
        Add(_armyGroupElement);

        _elementalElement = new ElementalElement(ArmyGroup.ArmyEntity.Element);
        Add(_elementalElement);

        _healthLabel = new Label($"Health: {armyGroup.ArmyEntity.Health}");
        _power = new Label($"Power: {armyGroup.ArmyEntity.Power}");
        _armor = new Label($"Armor: {armyGroup.ArmyEntity.Armor}");
        _attackRange = new Label($"Attack Range: {armyGroup.ArmyEntity.AttackRange}");
        _attackCooldown = new Label($"Attack Cooldown: {armyGroup.ArmyEntity.AttackCooldown}");
        _speed = new Label($"Speed: {armyGroup.ArmyEntity.Speed}");

        Add(_healthLabel);
        Add(_power);
        Add(_armor);
        Add(_attackRange);
        Add(_attackCooldown);
        Add(_speed);

        VisualElement spacer = new();
        spacer.AddToClassList(_ussCommonHorizontalSpacer);
        spacer.style.height = 50;
        spacer.style.backgroundImage = null;
        Add(spacer);

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

    public void Evolve(ArmyEntity entity)
    {
        _name.text += $" -> {entity.Name}";

        _armyGroupElement.Evolve(entity);

        _healthLabel.text += $" -> {ArmyGroup.ArmyEntity.Health}";
        _power.text += $" -> {ArmyGroup.ArmyEntity.Power}";
        _armor.text += $" -> {ArmyGroup.ArmyEntity.Armor}";
        _attackRange.text += $" -> {ArmyGroup.ArmyEntity.AttackRange}";
        _attackCooldown.text += $" -> {ArmyGroup.ArmyEntity.AttackCooldown}";
        _speed.text += $" -> {ArmyGroup.ArmyEntity.Speed}";
    }

    public void ResetKillsToEvolveBar()
    {
        _kills.SetValue(0);
        _killsToEvolve.SetValue(ArmyGroup.NumberOfKillsToEvolve());
    }
}
