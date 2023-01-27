using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingTroopsLimitElement : CampBuildingElement
{

    TroopsLimitElement _troopsLimitElement;
    CampBuildingTroopsLimit _cbTroopsLimit;

    public CampBuildingTroopsLimitElement(CampBuildingTroopsLimit campBuilding) : base(campBuilding)
    {
        _cbTroopsLimit = campBuilding;
    }

    protected override void AddUpgrade()
    {
        _troopsLimitElement = new TroopsLimitElement($"{_gameManager.TroopsLimit.ToString()}", 24);
        _upgradeContainer.Add(_troopsLimitElement);
    }

    protected override void SetUpgrade()
    {
        _troopsLimitElement.UpdateCountContainer(
            _gameManager.TroopsLimit.ToString()
            , Color.white);
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        int newTroopsLimit = _gameManager.TroopsLimit
                + _cbTroopsLimit.GetUpgradeByRank(_cbTroopsLimit.UpgradeRank + 1).LimitIncrease;
        _troopsLimitElement.UpdateCountContainer(newTroopsLimit.ToString(), Color.green);
    }

}
