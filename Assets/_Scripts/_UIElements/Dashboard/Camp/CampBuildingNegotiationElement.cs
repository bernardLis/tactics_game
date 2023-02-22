using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingNegotiationElement : CampBuildingElement
{


    public CampBuildingNegotiationElement(CampBuildingNegotiation campBuilding) : base(campBuilding)
    {
    }

    protected override void AddUpgrade()
    {
        _upgradeText.text = _campBuilding.TooltipText.Value;
        SetUpgrade();
    }

    protected override void SetUpgrade()
    {
        CampBuildingNegotiation c = (CampBuildingNegotiation)_campBuilding;
        _upgradeValue.text = c.GetUpgradeByRank(c.UpgradeRank).CursorSpeed.ToString();
        _upgradeValue.style.color = Color.white;
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        CampBuildingNegotiation c = (CampBuildingNegotiation)_campBuilding;
        _upgradeValue.text = c.GetUpgradeByRank(c.UpgradeRank + 1).CursorSpeed.ToString();
        _upgradeValue.style.color = Color.green;
    }

}
