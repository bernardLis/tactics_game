using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingHospitalElement : CampBuildingElement
{

    TextWithTooltip _upgradeText;

    public CampBuildingHospitalElement(CampBuildingHospital campBuilding) : base(campBuilding)
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
        CampBuildingHospital c = (CampBuildingHospital)_campBuilding;
        CampUpgradeHospital upgrade = c.GetUpgradeByRank(c.UpgradeRank);
        string labelText = $"{upgrade.PercentHealingImprovement}%";
        _upgradeText.UpdateText(labelText);
        _upgradeText.UpdateTextColor(Color.white);
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        CampBuildingHospital c = (CampBuildingHospital)_campBuilding;
        CampUpgradeHospital upgrade = c.GetUpgradeByRank(c.UpgradeRank + 1);
        string labelText = $"{upgrade.PercentHealingImprovement}%";
        _upgradeText.UpdateText(labelText);
        _upgradeText.UpdateTextColor(Color.green);
    }
}
