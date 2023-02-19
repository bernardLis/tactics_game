using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingSpiceRecyclerElement : CampBuildingElement
{

    TextWithTooltip _upgradeText;

    public CampBuildingSpiceRecyclerElement(CampBuildingSpiceRecycler campBuilding) : base(campBuilding)
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
        CampBuildingSpiceRecycler c = (CampBuildingSpiceRecycler)_campBuilding;
        _upgradeText.UpdateText(c.GetUpgradeByRank(c.UpgradeRank).ChanceToVisit.ToString());
        _upgradeText.UpdateTextColor(Color.white);
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        CampBuildingSpiceRecycler c = (CampBuildingSpiceRecycler)_campBuilding;
        _upgradeText.UpdateText(c.GetUpgradeByRank(c.UpgradeRank + 1).ChanceToVisit.ToString());
        _upgradeText.UpdateTextColor(Color.green);
    }

}
