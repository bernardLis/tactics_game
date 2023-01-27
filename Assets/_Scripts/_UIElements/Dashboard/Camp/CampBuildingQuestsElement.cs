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
        _betterQuestsRankElement = new(1, 0.5f, null, 5);
        Label l = new("Max Quest Rank: "); // HERE: could be a tooltip
        _upgradeContainer.style.flexDirection = FlexDirection.Column;
        _upgradeContainer.style.alignItems = Align.Center;

        _upgradeContainer.Add(l);
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
