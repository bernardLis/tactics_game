using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingCard : TooltipCard
{
    const string _ussClassName = "building-card__";
    const string _ussMain = _ussClassName + "main";
    const string _ussIcon = _ussClassName + "icon";

    protected ElementalElement _elementalElement;
    protected Label _nameLabel;
    protected Label _levelLabel;

    Building _building;

    public BuildingCard(Building building)
    {
        Initialize();

        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BuildingCardStyles);
        if (ss != null) styleSheets.Add(ss);
        AddToClassList(_ussMain);

        _building = building;
        PopulateCard();
    }

    protected virtual void PopulateCard()
    {
        HandleIcon();
        HandleNameLabel();
        HandleLevelLabel();
    }

    protected virtual void HandleIcon()
    {
        VisualElement icon = new();
        icon.AddToClassList(_ussIcon);
        icon.style.backgroundImage = new StyleBackground(_building.Icon);
        _topLeftContainer.Add(icon);
    }

    protected virtual void HandleNameLabel()
    {
        _nameLabel = new(Helpers.ParseScriptableObjectName(_building.name));
        _nameLabel.AddToClassList(_ussName);
        _topRightContainer.Add(_nameLabel);
    }

    protected virtual void HandleLevelLabel()
    {
        _levelLabel = new();
        _levelLabel.text = $"Level {_building.CurrentLevel.Value}";
        _topRightContainer.Add(_levelLabel);

        _building.CurrentLevel.OnValueChanged += (i) =>
        {
            _levelLabel.text = $"Level {i}";
        };
    }
}
