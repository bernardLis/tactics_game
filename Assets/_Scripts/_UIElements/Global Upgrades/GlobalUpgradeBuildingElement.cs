using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class GlobalUpgradeBuildingElement : GlobalUpgradeElement
{

    public GlobalUpgradeBuildingElement(GlobalUpgrade gu) : base(gu)
    {

    }

    protected override void DisplayTooltip()
    {
        _tooltipElement = new();

        if (GlobalUpgrade.CurrentLevel == -1)
        {
            _tooltipElement = new Label($"Unlock {GlobalUpgrade.name}");
        }
        else
        {
            GlobalUpgradeLevelBuilding level = (GlobalUpgradeLevelBuilding)GlobalUpgrade.GetCurrentLevel();
            _tooltipElement.Add(new Label($"Max creatures: {level.ProductionLimit}"));
            _tooltipElement.Add(new Label($"Production delay: {level.ProductionDelay}"));
        }

        base.DisplayTooltip();
    }
}
