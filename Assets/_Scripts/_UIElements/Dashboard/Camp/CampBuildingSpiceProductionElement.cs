using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingSpiceProductionElement : CampBuildingElement
{
    SpiceElement _spicePerWeekSpiceElement;

    public CampBuildingSpiceProductionElement(CampBuildingSpiceProduction campBuilding) : base(campBuilding)
    {
    }

    protected override void AddUpgrade()
    {
        _spicePerWeekSpiceElement = new(0);
        _upgradeText.text = _campBuilding.TooltipText.Value;
        _upgradeValue.style.display = DisplayStyle.None;
        _upgradeContainer.Add(_spicePerWeekSpiceElement);
        SetUpgrade();
    }

    protected override void SetUpgrade()
    {
        CampBuildingSpiceProduction c = (CampBuildingSpiceProduction)_campBuilding;
        _spicePerWeekSpiceElement.ChangeAmount(c.GetUpgradeByRank(c.UpgradeRank).SpicePerWeek);
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        CampBuildingSpiceProduction c = (CampBuildingSpiceProduction)_campBuilding;
        _spicePerWeekSpiceElement.ChangeAmount(c.GetUpgradeByRank(c.UpgradeRank + 1).SpicePerWeek);
    }

}
