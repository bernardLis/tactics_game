using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingQuestInfoElement : CampBuildingElement
{
    public CampBuildingQuestInfoElement(CampBuildingQuestInfo campBuilding) : base(campBuilding)
    {
    }

    protected override void AddUpgrade()
    {
        _upgradeText.text = _campBuilding.TooltipText.Value;
        SetUpgrade();
    }

    protected override void SetUpgrade()
    {
        CampBuildingQuestInfo c = (CampBuildingQuestInfo)_campBuilding;
        _upgradeValue.text = c.GetUpgradeByRank(c.UpgradeRank).RevealedInfo;
        _upgradeValue.style.color = Color.white;
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        CampBuildingQuestInfo c = (CampBuildingQuestInfo)_campBuilding;
        _upgradeValue.text = c.GetUpgradeByRank(c.UpgradeRank + 1).RevealedInfo;
        _upgradeValue.style.color = Color.green;
    }

}
