using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingHospitalElement : CampBuildingElement
{

    public CampBuildingHospitalElement(CampBuildingHospital campBuilding) : base(campBuilding)
    {
    }

    protected override void AddUpgrade()
    {
        _upgradeText.text = _campBuilding.TooltipText.Value;
        SetUpgrade();
    }

    protected override void SetUpgrade()
    {
        CampBuildingHospital c = (CampBuildingHospital)_campBuilding;
        CampUpgradeHospital upgrade = c.GetUpgradeByRank(c.UpgradeRank);
        string labelText = $"{upgrade.PercentHealingImprovement}%";
        _upgradeValue.text = labelText;
        _upgradeValue.style.color = Color.white;
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        CampBuildingHospital c = (CampBuildingHospital)_campBuilding;
        CampUpgradeHospital upgrade = c.GetUpgradeByRank(c.UpgradeRank + 1);
        string labelText = $"{upgrade.PercentHealingImprovement}%";
        _upgradeValue.text = labelText;
        _upgradeValue.style.color = Color.green;
    }
}
