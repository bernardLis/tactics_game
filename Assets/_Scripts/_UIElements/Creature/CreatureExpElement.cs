using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class CreatureExpElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary-black";
    const string _ussCommonHorizontalSpacer = "common__horizontal-spacer";

    const string _ussClassName = "creature-exp__";
    const string _ussMain = _ussClassName + "main";

    GameManager _gameManager;

    public Creature Creature;

    Label _name;
    public CreatureIcon CreatureIcon;

    int _availableKills;
    IntVariable _kills;
    IntVariable _killsToEvolve;

    public CreatureExpElement(Creature creature)
    {
        _gameManager = GameManager.Instance;
        var common = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (common != null)
            styleSheets.Add(common);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CreatureExpElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Creature = creature;

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        _name = new(creature.Name);
        Add(_name);

        CreatureIcon = new(creature);
        Add(CreatureIcon);

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

}
