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

    protected VisualElement _infoContainer;

    protected LockOverlayElement _lockElement;

    protected Building _building;

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
        HandleBuildingInfoContainer();
        HandleBuildingSecured();
    }

    protected virtual void HandleIcon()
    {
    }

    protected virtual void HandleNameLabel()
    {
        _nameLabel = new(Helpers.ParseScriptableObjectName(_building.name));
        _nameLabel.AddToClassList(_ussName);
        _topRightContainer.Add(_nameLabel);
    }

    protected virtual void HandleBuildingInfoContainer()
    {
    }

    void HandleBuildingSecured()
    {
        if (_building.IsSecured) return;
        Label txt = new("Secure tile to unlock.");
        _lockElement = new(txt);
        _middleContainer.Add(_lockElement);
        _building.OnSecured += () =>
        {
            _lockElement.Unlock();
        };
    }
}
