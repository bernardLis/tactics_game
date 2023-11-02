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

    VisualElement _infoContainer;

    LockOverlayElement _lockElement;

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
        HandleBuildingInfoContainer();
        HandleBuildingSecured();
    }

    protected virtual void HandleIcon()
    {
        EntityIcon entityIcon = new(_building.GetCurrentUpgrade().ProducedCreature);
        _topLeftContainer.Add(entityIcon);
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

    void HandleBuildingInfoContainer()
    {
        _infoContainer = new();
        _middleContainer.Add(_infoContainer);

        Label wolfCountLabel = new($"Max: {_building.GetCurrentUpgrade().ProductionLimit}");
        _infoContainer.Add(wolfCountLabel);
        _building.OnUpgradePurchased += () =>
        {
            wolfCountLabel.text = $"Max: {_building.GetCurrentUpgrade().ProductionLimit}";
        };

        Label delay = new($"Respawn: {_building.GetCurrentUpgrade().ProductionDelay}s");
        _infoContainer.Add(delay);
        _building.OnUpgradePurchased += () =>
        {
            delay.text = $"Respawn: {_building.GetCurrentUpgrade().ProductionDelay}s";
        };
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
