using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingProductionCard : BuildingCard
{
    BuildingProduction _buildingProduction;
    GoldElement _upgradeCostElement;

    public BuildingProductionCard(Building building) : base(building)
    {
    }

    protected override void PopulateCard()
    {
        _buildingProduction = _building as BuildingProduction;

        base.PopulateCard();
        HandleLevelLabel();
    }

    protected override void HandleIcon()
    {
        EntityIcon entityIcon = new(_buildingProduction.GetCurrentUpgrade().ProducedCreature);
        _topLeftContainer.Add(entityIcon);
    }

    void HandleLevelLabel()
    {
        _levelLabel = new();
        _levelLabel.text = $"Level {_buildingProduction.CurrentLevel.Value}";
        _topRightContainer.Add(_levelLabel);

        _buildingProduction.CurrentLevel.OnValueChanged += (i) =>
        {
            _levelLabel.text = $"Level {i}";
        };
    }

    protected override void HandleBuildingInfoContainer()
    {
        _infoContainer = new();
        _middleContainer.Add(_infoContainer);

        Label limitLabel = new($"Max: {_buildingProduction.GetCurrentUpgrade().ProductionLimit}");
        _infoContainer.Add(limitLabel);
        _buildingProduction.OnUpgradePurchased += () =>
        {
            limitLabel.text = $"Max: {_buildingProduction.GetCurrentUpgrade().ProductionLimit}";
        };

        Label delayLabel = new($"Respawn: {_buildingProduction.GetCurrentUpgrade().ProductionDelay}s");
        _infoContainer.Add(delayLabel);
        _buildingProduction.OnUpgradePurchased += () =>
        {
            delayLabel.text = $"Respawn: {_buildingProduction.GetCurrentUpgrade().ProductionDelay}s";
        };

        HandleUpgradeCost();
    }

    void HandleUpgradeCost()
    {
        if (_buildingProduction.GetNextUpgrade() == null) return;

        _upgradeCostElement = new GoldElement(_buildingProduction.GetNextUpgrade().Cost);
        _infoContainer.Add(_upgradeCostElement);
        _buildingProduction.OnUpgradePurchased += () =>
        {
            if (_buildingProduction.GetNextUpgrade() == null)
            {
                _infoContainer.Remove(_upgradeCostElement);
                return;
            }
            _upgradeCostElement.ChangeAmount(_buildingProduction.GetNextUpgrade().Cost);
        };
    }

}
