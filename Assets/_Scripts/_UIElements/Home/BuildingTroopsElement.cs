using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BuildingTroopsElement : FullScreenElement
{
    const string _ussClassName = "building-troops__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTreeContainer = _ussClassName + "tree-container";
    const string _ussArrowLabel = _ussClassName + "arrow-label";

    const string _ussTitle = _ussClassName + "title";
    const string _ussUpgradesContainer = _ussClassName + "upgrades-container";

    BuildingTroops _building;

    VisualElement _container;

    VisualElement _topContainer;
    VisualElement _middleContainer;
    VisualElement _bottomContainer;

    Label _creatureTierLabel;
    BuildingUpgradeTreeElement _creatureTierTreeElement;

    TroopsLimitElement _troopsLimitElement;
    BuildingUpgradeTreeElement _maxTroopsTreeElement;

    public BuildingTroopsElement(BuildingTroops building) : base()
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BuildingTroopsStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _building = building;

        _container = new();
        _container.AddToClassList(_ussMain);
        _content.Add(_container);

        AddCreatureTierLabel();
        _topContainer = new();
        _topContainer.AddToClassList(_ussTreeContainer);
        _container.Add(_topContainer);

        AddTroopsLimitElement();
        _middleContainer = new();
        _middleContainer.AddToClassList(_ussTreeContainer);
        _container.Add(_middleContainer);

        _bottomContainer = new();
        _container.Add(_bottomContainer);

        _creatureTierTreeElement = new(_building.CreatureTierTree);
        _topContainer.Add(_creatureTierTreeElement);

        _maxTroopsTreeElement = new(_building.MaxTroopsTree);
        _maxTroopsTreeElement.OnUpgradePurchased += (d) => UpdateTroopsLimit();
        _middleContainer.Add(_maxTroopsTreeElement);

        AddContinueButton();
    }

    void AddCreatureTierLabel()
    {
        VisualElement container = new();
        container.style.width = Length.Percent(100);
        container.style.alignItems = Align.Center;
        _container.Add(container);

        _creatureTierLabel = new($"Creature tier: {0}");
        _creatureTierLabel.AddToClassList(_ussTitle);
        container.Add(_creatureTierLabel);
        _building.CreatureTierTree.CurrentValue.OnValueChanged += UpdateCreatureTierLabel;
        UpdateCreatureTierLabel(default);
    }

    void UpdateCreatureTierLabel(int bla)
    {
        _creatureTierLabel.text = $"Creature tier: {_building.CreatureTierTree.CurrentValue.Value}";
    }

    void AddTroopsLimitElement()
    {
        VisualElement container = new();
        container.style.width = Length.Percent(100);
        container.style.alignItems = Align.Center;
        _container.Add(container);

        _troopsLimitElement = new($"{0}/{0}", 36);
        container.Add(_troopsLimitElement);
        UpdateTroopsLimit();
    }

    void UpdateTroopsLimit()
    {
        // _troopsLimitElement.UpdateCountContainer(
        //         $"{_gameManager.PlayerHero.CreatureArmy.Count}/{_building.MaxTroopsTree.CurrentValue.Value}"
        //         , Color.white);
    }
}
