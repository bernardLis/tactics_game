using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingItemProductionElement : CampBuildingElement
{

    TextWithTooltip _upgradeText;

    public CampBuildingItemProductionElement(CampBuildingItemProduction campBuilding) : base(campBuilding)
    {
    }

    protected override void AddUpgrade()
    {
        _upgradeText = new("TXT", _campBuilding.TooltipText.Value);
        SetUpgrade();
        _upgradeText.UpdateFontSize(36);
        _upgradeContainer.Add(_upgradeText);
    }

    protected override void SetUpgrade()
    {
        CampBuildingItemProduction c = (CampBuildingItemProduction)_campBuilding;
        _upgradeText.UpdateText(c.GetUpgradeByRank(c.UpgradeRank).DailyChanceToProduceItem.ToString());
        _upgradeText.UpdateTextColor(Color.white);
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        CampBuildingItemProduction c = (CampBuildingItemProduction)_campBuilding;
        _upgradeText.UpdateText(c.GetUpgradeByRank(c.UpgradeRank + 1).DailyChanceToProduceItem.ToString());
        _upgradeText.UpdateTextColor(Color.green);
    }
}
