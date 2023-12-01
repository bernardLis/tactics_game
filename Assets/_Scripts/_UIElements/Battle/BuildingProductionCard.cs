using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingProductionCard : BuildingCard
{
    BuildingProduction _buildingProduction;

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
        EntityIcon entityIcon = new(_buildingProduction.ProducedCreature);
        _topLeftContainer.Add(entityIcon);
    }

    void HandleLevelLabel()
    {
        _levelLabel = new();
        _levelLabel.text = $"Level {_buildingProduction.BuildingUpgrade.CurrentLevel + 1}";
        _topRightContainer.Add(_levelLabel);

    }

    protected override void HandleBuildingInfoContainer()
    {
        _infoContainer = new();
        _middleContainer.Add(_infoContainer);

        GlobalUpgradeLevelBuilding currentUpgrade = _buildingProduction.GetCurrentUpgrade();

        Label limitLabel = new($"Max: {currentUpgrade.ProductionLimit}");
        _infoContainer.Add(limitLabel);

        Label delayLabel = new($"Respawn: {currentUpgrade.ProductionDelay}s");
        _infoContainer.Add(delayLabel);
    }
}
