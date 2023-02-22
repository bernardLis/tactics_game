using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingItemProductionElement : CampBuildingElement
{
    public CampBuildingItemProductionElement(CampBuildingItemProduction campBuilding) : base(campBuilding)
    {
    }

    protected override void AddUpgrade()
    {
        _upgradeText.text = _campBuilding.TooltipText.Value;
        SetUpgrade();
    }

    protected override void SetUpgrade()
    {
        CampBuildingItemProduction c = (CampBuildingItemProduction)_campBuilding;
        _upgradeValue.text = c.GetUpgradeByRank(c.UpgradeRank).DailyChanceToProduceItem.ToString();
        _upgradeValue.style.color = Color.white;

    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        CampBuildingItemProduction c = (CampBuildingItemProduction)_campBuilding;
        _upgradeValue.text = c.GetUpgradeByRank(c.UpgradeRank + 1).DailyChanceToProduceItem.ToString();
        _upgradeValue.style.color = Color.green;
    }
}
