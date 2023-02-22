using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingRecruitingElement : CampBuildingElement
{
    public CampBuildingRecruitingElement(CampBuildingRecruiting campBuilding) : base(campBuilding)
    {
    }

    protected override void AddUpgrade()
    {
        _upgradeText.text = _campBuilding.TooltipText.Value;
        SetUpgrade();
    }

    protected override void SetUpgrade()
    {
        CampBuildingRecruiting c = (CampBuildingRecruiting)_campBuilding;
        CampUpgradeRecruiting upgrade = c.GetUpgradeByRank(c.UpgradeRank);
        _upgradeValue.text = $"{upgrade.MaxRecruitLevel}";
        _upgradeValue.style.color = Color.white;
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        CampBuildingRecruiting c = (CampBuildingRecruiting)_campBuilding;
        CampUpgradeRecruiting upgrade = c.GetUpgradeByRank(c.UpgradeRank + 1);
        _upgradeValue.text = $"{upgrade.MaxRecruitLevel}";
        _upgradeValue.style.color = Color.green;
    }

}
