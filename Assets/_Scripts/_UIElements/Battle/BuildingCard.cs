using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingCard : TooltipCard
{
    const string _ussClassName = "building-card__";
    const string _ussMain = _ussClassName + "main";

    public BuildingCard(Building building)
    {
        Initialize();

        // var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleTooltipCardStyles);
        // if (ss != null) styleSheets.Add(ss);
        // AddToClassList(_ussMain);

    }

}
