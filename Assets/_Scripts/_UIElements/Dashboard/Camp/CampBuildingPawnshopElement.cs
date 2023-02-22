using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingPawnshopElement : CampBuildingElement
{
    public CampBuildingPawnshopElement(CampBuildingPawnshop campBuilding) : base(campBuilding)
    {
    }

    protected override void AddUpgrade()
    {
        _upgradeText.text = _campBuilding.TooltipText.Value;
        SetUpgrade();
    }

    protected override void SetUpgrade()
    {
        CampBuildingPawnshop c = (CampBuildingPawnshop)_campBuilding;
        _upgradeValue.text = c.GetUpgradeByRank(c.UpgradeRank).ChanceToVisit.ToString();
        _upgradeValue.style.color = Color.white;
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        CampBuildingPawnshop c = (CampBuildingPawnshop)_campBuilding;
        _upgradeValue.text = c.GetUpgradeByRank(c.UpgradeRank + 1).ChanceToVisit.ToString();
        _upgradeValue.style.color = Color.green;
    }

}
