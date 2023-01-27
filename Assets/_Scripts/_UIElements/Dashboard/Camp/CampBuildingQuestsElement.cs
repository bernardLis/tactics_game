using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingQuestsElement : CampBuildingElement
{

    CampBuildingQuests _cbQuests;
    StarRankElement _betterQuestsRankElement;

    public CampBuildingQuestsElement(CampBuildingQuests campBuilding) : base(campBuilding)
    {
        _cbQuests = campBuilding;
    }

    protected override void AddUpgrade()
    {
        _betterQuestsRankElement = new(_cbQuests.GetUpgradeByRank(_cbQuests.UpgradeRank).MaxQuestRank, 0.5f, null, 5);
        Label l = new("Max Quest Rank: "); // HERE: could be a tooltip
        _upgradeContainer.style.flexDirection = FlexDirection.Column;
        _upgradeContainer.style.alignItems = Align.Center;

        _upgradeContainer.Add(l);
        _upgradeContainer.Add(_betterQuestsRankElement);
    }

    protected override void SetUpgrade()
    {
        _betterQuestsRankElement.SetRank(_cbQuests.GetUpgradeByRank(_cbQuests.UpgradeRank).MaxQuestRank);
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        _betterQuestsRankElement.SetRank(_cbQuests.GetUpgradeByRank(_cbQuests.UpgradeRank + 1).MaxQuestRank);
    }

}
