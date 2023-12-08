using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class GlobalUpgradeElementTooltip : VisualElement
{
    const string _ussCommonSpacer = "common__horizontal-spacer";

    GlobalUpgrade _globalUpgrade;


    public GlobalUpgradeElementTooltip(GlobalUpgrade globalUpgrade)
    {
        _globalUpgrade = globalUpgrade;
        _globalUpgrade.OnLevelChanged += HandleTooltip;

        HandleTooltip();
    }

    void HandleTooltip()
    {
        Clear();
        if (_globalUpgrade.Type == GlobalUpgradeType.Building)
        {
            CreateBuildingTooltip();
            return;
        }

        CreateTooltip();
    }

    void CreateTooltip()
    {
        Add(new Label(_globalUpgrade.Description));
        VisualElement spacer = new();
        spacer.AddToClassList(_ussCommonSpacer);
        Add(spacer);

        Add(new Label("Current:"));
        if (_globalUpgrade.CurrentLevel == -1)
            Add(new Label("Not unlocked"));
        else
            Add(new Label(_globalUpgrade.GetCurrentLevel().Description));

        VisualElement spacer1 = new();
        spacer1.AddToClassList(_ussCommonSpacer);
        Add(spacer1);

        Add(new Label("Next:"));
        if (_globalUpgrade.IsMaxLevel())
            Add(new Label("Max level reached"));
        else
            Add(new Label(_globalUpgrade.GetNextLevel().Description));

    }

    void CreateBuildingTooltip()
    {
        Add(new Label(_globalUpgrade.name));
        VisualElement spacer = new();
        spacer.AddToClassList(_ussCommonSpacer);
        Add(spacer);

        Add(new Label("Current:"));
        if (_globalUpgrade.CurrentLevel == -1)
            Add(new Label("Not unlocked"));
        else
        {
            GlobalUpgradeLevelBuilding level = (GlobalUpgradeLevelBuilding)_globalUpgrade.GetCurrentLevel();
            Add(new Label($"Max creatures: {level.ProductionLimit}"));
            Add(new Label($"Production delay: {level.ProductionDelay}"));
        }

        VisualElement spacer1 = new();
        spacer1.AddToClassList(_ussCommonSpacer);
        Add(spacer1);

        Add(new Label("Next:"));
        if (_globalUpgrade.IsMaxLevel())
            Add(new Label("Max level reached"));
        else
        {
            GlobalUpgradeLevelBuilding level = (GlobalUpgradeLevelBuilding)_globalUpgrade.GetNextLevel();
            Add(new Label($"Max creatures: {level.ProductionLimit}"));
            Add(new Label($"Production delay: {level.ProductionDelay}"));

        }
    }
}
