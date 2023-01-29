using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingQuestInfoElement : CampBuildingElement
{

    TextWithTooltip _upgradeText;

    public CampBuildingQuestInfoElement(CampBuildingQuestInfo campBuilding) : base(campBuilding)
    {
    }

    protected override void AddUpgrade()
    {
        string tooltipText = "Each upgrade reveals more information.";
        _upgradeText = new("TXT", tooltipText);
        SetUpgrade();
        _upgradeText.UpdateFontSize(16);
        _upgradeContainer.Add(_upgradeText);
    }

    protected override void SetUpgrade()
    {
        CampBuildingQuestInfo c = (CampBuildingQuestInfo)_campBuilding;
        _upgradeText.UpdateText(c.GetUpgradeByRank(c.UpgradeRank).RevealedInfo);
        _upgradeText.UpdateTextColor(Color.white);
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        CampBuildingQuestInfo c = (CampBuildingQuestInfo)_campBuilding;
        _upgradeText.UpdateText(c.GetUpgradeByRank(c.UpgradeRank + 1).RevealedInfo);
        _upgradeText.UpdateTextColor(Color.green);
    }

}
