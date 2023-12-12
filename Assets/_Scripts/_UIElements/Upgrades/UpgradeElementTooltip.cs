using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class UpgradeElementTooltip : VisualElement
{
    const string _ussCommonSpacer = "common__horizontal-spacer";

    Upgrade _upgrade;


    public UpgradeElementTooltip(Upgrade upgrade)
    {
        _upgrade = upgrade;
        _upgrade.OnLevelChanged += HandleTooltip;

        HandleTooltip();
    }

    void HandleTooltip()
    {
        Clear();
        if (_upgrade.Type == UpgradeType.Building)
        {
            CreateBuildingTooltip();
            return;
        }

        CreateTooltip();
    }

    void CreateTooltip()
    {
        Add(new Label(_upgrade.Description));
        VisualElement spacer = new();
        spacer.AddToClassList(_ussCommonSpacer);
        Add(spacer);

        Add(new Label("Current:"));
        if (_upgrade.CurrentLevel == -1)
            Add(new Label("Not unlocked"));
        else
            Add(new Label(_upgrade.GetCurrentLevel().Description));

        VisualElement spacer1 = new();
        spacer1.AddToClassList(_ussCommonSpacer);
        Add(spacer1);

        Add(new Label("Next:"));
        if (_upgrade.IsMaxLevel())
            Add(new Label("Max level reached"));
        else
            Add(new Label(_upgrade.GetNextLevel().Description));

    }

    void CreateBuildingTooltip()
    {
        Add(new Label(_upgrade.name));
        VisualElement spacer = new();
        spacer.AddToClassList(_ussCommonSpacer);
        Add(spacer);

        Add(new Label("Current:"));
        if (_upgrade.CurrentLevel == -1)
            Add(new Label("Not unlocked"));
        else
        {
            UpgradeLevelBuilding level = (UpgradeLevelBuilding)_upgrade.GetCurrentLevel();
            Add(new Label($"Max creatures: {level.ProductionLimit}"));
            Add(new Label($"Production delay: {level.ProductionDelay}"));
        }

        VisualElement spacer1 = new();
        spacer1.AddToClassList(_ussCommonSpacer);
        Add(spacer1);

        Add(new Label("Next:"));
        if (_upgrade.IsMaxLevel())
            Add(new Label("Max level reached"));
        else
        {
            UpgradeLevelBuilding level = (UpgradeLevelBuilding)_upgrade.GetNextLevel();
            Add(new Label($"Max creatures: {level.ProductionLimit}"));
            Add(new Label($"Production delay: {level.ProductionDelay}"));
        }
    }
}
