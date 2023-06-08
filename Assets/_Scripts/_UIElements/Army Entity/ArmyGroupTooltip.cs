using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class ArmyGroupTooltip : VisualElement
{
    const string _ussClassName = "army-group-tooltip__";
    const string _ussMain = _ussClassName + "main";

    GameManager _gameManager;

    ArmyGroup _armyGroup;

    public ArmyGroupTooltip(ArmyGroup armyGroup)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ArmyGroupTooltipStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _armyGroup = armyGroup;

        Label count = new($"Number of units: {_armyGroup.NumberOfUnits}");
        Add(count);

        CreatureElement creatureElement = new(_armyGroup.Creature);
        Add(creatureElement);
        creatureElement.style.marginBottom = 20;

        AddKillsToEvolveBar();

        Label damageDealt = new($"Damage dealt: {_armyGroup.TotalDamageDealt}");
        Add(damageDealt);

        Label damageTaken = new($"Damage taken: {_armyGroup.TotalDamageTaken}");
        Add(damageTaken);
        damageTaken.style.marginBottom = 20;
    }

    void AddKillsToEvolveBar()
    {
        IntVariable kills = ScriptableObject.CreateInstance<IntVariable>();
        IntVariable killsToEvolve = ScriptableObject.CreateInstance<IntVariable>();
        kills.SetValue(_armyGroup.TotalKillCount);
        killsToEvolve.SetValue(_armyGroup.NumberOfKillsToEvolve());

        Add(new Label("Kills to evolve:"));

        Color barColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        ResourceBarElement killBar = new(barColor, "Kills", kills, killsToEvolve);
        killBar.style.marginBottom = 20;
        killBar.style.width = 200;

        Add(killBar);
    }
}
