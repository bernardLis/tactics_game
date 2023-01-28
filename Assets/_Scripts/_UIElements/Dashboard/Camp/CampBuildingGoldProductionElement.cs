using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingGoldProductionElement : CampBuildingElement
{

    GoldElement _goldPerWeekGoldElement;

    public CampBuildingGoldProductionElement(CampBuildingGoldProduction campBuilding) : base(campBuilding)
    {
    }

    protected override void AddUpgrade()
    {
        _goldPerWeekGoldElement = new(0);
        TooltipElement tt = new(_goldPerWeekGoldElement, new Label("Gold produced per week"));
        _goldPerWeekGoldElement.AddTooltip(tt);

        SetUpgrade();
        _upgradeContainer.Add(_goldPerWeekGoldElement);
    }

    protected override void SetUpgrade()
    {
        CampBuildingGoldProduction c = (CampBuildingGoldProduction)_campBuilding;
        _goldPerWeekGoldElement.ChangeAmount(c.GetUpgradeByRank(c.UpgradeRank).GoldPerWeek);
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        CampBuildingGoldProduction c = (CampBuildingGoldProduction)_campBuilding;
        _goldPerWeekGoldElement.ChangeAmount(c.GetUpgradeByRank(c.UpgradeRank + 1).GoldPerWeek);
    }

}
