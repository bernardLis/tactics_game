using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingRecruitingElement : CampBuildingElement
{

    TextWithTooltip _upgradeText;

    public CampBuildingRecruitingElement(CampBuildingRecruiting campBuilding) : base(campBuilding)
    {
    }

    protected override void AddUpgrade()
    {
        string tooltipText = "Max new recruit level.";
        _upgradeText = new("TXT", tooltipText);
        SetUpgrade();
        _upgradeText.UpdateFontSize(36);
        _upgradeContainer.Add(_upgradeText);
    }

    protected override void SetUpgrade()
    {
        CampBuildingRecruiting c = (CampBuildingRecruiting)_campBuilding;
        CampUpgradeRecruiting upgrade = c.GetUpgradeByRank(c.UpgradeRank);
        string labelText = $"{upgrade.MaxRecruitLevel}";
        _upgradeText.UpdateText(labelText);
        _upgradeText.UpdateTextColor(Color.white);
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        CampBuildingRecruiting c = (CampBuildingRecruiting)_campBuilding;
        CampUpgradeRecruiting upgrade = c.GetUpgradeByRank(c.UpgradeRank + 1);
        string labelText = $"{upgrade.MaxRecruitLevel}";
        _upgradeText.UpdateText(labelText);
        _upgradeText.UpdateTextColor(Color.green);
    }

}
