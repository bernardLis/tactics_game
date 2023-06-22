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

    const string _ussClassName = "creature-evolution__";
    const string _ussMain = _ussClassName + "main";
    const string _ussName = _ussClassName + "name";
    const string _ussAbilityContainer = _ussClassName + "ability-container";

    GameManager _gameManager;

    public Creature Creature;

    Label _name;
    CreatureElement _creatureElement;

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

    public CreatureEvolutionElement(Creature creature)
    {
        _gameManager = GameManager.Instance;
        var common = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (common != null)
            styleSheets.Add(common);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CreatureEvolutionElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Creature = creature;

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        _name = new(creature.Name);
        _name.AddToClassList(_ussName);
        Add(_name);

        _creatureElement = new(creature);
        _creatureElement.LargeIcon();
        Add(_creatureElement);

        _elementalElement = new ElementalElement(Creature.Element);
        Add(_elementalElement);

        _healthLabel = new Label($"Health: {creature.GetHealth()}");
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

        _availableKills = Creature.TotalKillCount - Creature.OldKillCount;
        _kills = ScriptableObject.CreateInstance<IntVariable>();
        _killsToEvolve = ScriptableObject.CreateInstance<IntVariable>();
        _kills.SetValue(Creature.OldKillCount);
        //  _killsToEvolve.SetValue(Creature.KillsToUpgrade);
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
        //    int killChange = Mathf.Clamp(_availableKills, 0, Creature.KillsToUpgrade);
        //     _availableKills -= killChange;
        _killsThisBattleElement.ChangeAmount(_availableKills);
        //      _kills.ApplyChange(killChange);
    }

    public void Evolve(Creature creature)
    {
        _name.text += $" -> {creature.Name}";

        _creatureElement.Evolve(creature);

        _healthLabel.text += $" -> {creature.GetHealth()}";
        _power.text += $" -> {creature.GetPower()}";
        _armor.text += $" -> {creature.Armor}";
        _attackRange.text += $" -> {creature.AttackRange}";
        _attackCooldown.text += $" -> {creature.AttackCooldown}";
        _speed.text += $" -> {creature.Speed}";

        _abilityContainer.Add(new Label(" -> "));
        if (creature.CreatureAbility != null)
            _abilityContainer.Add(new CreatureAbilityElement(creature.CreatureAbility));

    }

    public void ResetKillsToEvolveBar()
    {
        //     _killsToEvolve.SetValue(Creature.KillsToUpgrade);
    }
}
