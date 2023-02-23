using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingTroopsLimitElement : CampBuildingElement
{

    TroopsLimitElement _troopsLimitElement;

    public CampBuildingTroopsLimitElement(CampBuildingTroopsLimit campBuilding) : base(campBuilding)
    {
    }

    protected override void AddUpgrade()
    {
        _troopsLimitElement = new TroopsLimitElement("0", 24);
        _upgradeText.style.display = DisplayStyle.None;
        _upgradeValue.style.display = DisplayStyle.None;
        SetUpgrade();
        _upgradeContainer.Add(_troopsLimitElement);
    }

    protected override void SetUpgrade()
    {
        CampBuildingTroopsLimit c = (CampBuildingTroopsLimit)_campBuilding;
        _troopsLimitElement.UpdateCountContainer(
            $"{c.GetUpgradeByRank(c.UpgradeRank).TroopsLimit}"
            , Color.white);
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        CampBuildingTroopsLimit c = (CampBuildingTroopsLimit)_campBuilding;
        int newTroopsLimit = c.GetUpgradeByRank(c.UpgradeRank + 1).TroopsLimit;
        _troopsLimitElement.UpdateCountContainer(newTroopsLimit.ToString(), Color.green);
    }

}
