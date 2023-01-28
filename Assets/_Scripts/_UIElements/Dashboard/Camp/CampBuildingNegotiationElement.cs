using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingNegotiationElement : CampBuildingElement
{

    TextWithTooltip _upgradeText;

    public CampBuildingNegotiationElement(CampBuildingNegotiation campBuilding) : base(campBuilding)
    {
    }

    protected override void AddUpgrade()
    {
        string tooltipText = "Cursor speed when negotiating wages.";
        _upgradeText = new("TXT", tooltipText);
        SetUpgrade();
        _upgradeText.UpdateFontSize(36);
        _upgradeContainer.Add(_upgradeText);
    }

    protected override void SetUpgrade()
    {
        CampBuildingNegotiation c = (CampBuildingNegotiation)_campBuilding;
        _upgradeText.UpdateText(c.GetUpgradeByRank(c.UpgradeRank).CursorSpeed.ToString());
        _upgradeText.UpdateTextColor(Color.white);
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        CampBuildingNegotiation c = (CampBuildingNegotiation)_campBuilding;
        _upgradeText.UpdateText(c.GetUpgradeByRank(c.UpgradeRank + 1).CursorSpeed.ToString());
        _upgradeText.UpdateTextColor(Color.green);
    }

}
