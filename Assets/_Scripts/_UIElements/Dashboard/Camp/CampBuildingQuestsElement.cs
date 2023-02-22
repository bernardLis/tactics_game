using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingQuestsElement : CampBuildingElement
{

    StarRankElement _betterQuestsRankElement;

    public CampBuildingQuestsElement(CampBuildingQuests campBuilding) : base(campBuilding)
    {
    }

    protected override void AddUpgrade()
    {
        _upgradeText.text = _campBuilding.TooltipText.Value;
        _betterQuestsRankElement = new(1, 0.5f, null, 5);
        _upgradeContainer.Add(_betterQuestsRankElement);
        SetUpgrade();
    }

    protected override void SetUpgrade()
    {
        CampBuildingQuests c = (CampBuildingQuests)_campBuilding;
        _betterQuestsRankElement.SetRank(c.GetUpgradeByRank(c.UpgradeRank).MaxQuestRank);
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        CampBuildingQuests c = (CampBuildingQuests)_campBuilding;
        _betterQuestsRankElement.SetRank(c.GetUpgradeByRank(c.UpgradeRank + 1).MaxQuestRank);
    }

}
