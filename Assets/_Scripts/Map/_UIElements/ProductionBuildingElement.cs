using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProductionBuildingElement : BuildingElement
{
    const string _ussClassName = "production-building__";
    const string _ussMain = _ussClassName + "main";

    public ArmyBuyableElement ArmyBuyableElement { get; private set; }

    ProductionBuilding _productionBuilding;

    public ProductionBuildingElement(ProductionBuilding building) : base(building)
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ProductionBuildingElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _productionBuilding = building;

        ArmyBuyableElement = new ArmyBuyableElement(building);

        Add(ArmyBuyableElement);
    }

}
