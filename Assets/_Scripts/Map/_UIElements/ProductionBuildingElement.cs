using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProductionBuildingElement : BuildingElement
{
    const string _ussClassName = "production-building__";
    const string _ussMain = _ussClassName + "main";

    ArmySlotElement _armySlotElement;

    ProductionBuilding _productionBuilding;

    public ProductionBuildingElement(ProductionBuilding building) : base(building)
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ProductionBuildingElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _productionBuilding = building;
        _productionBuilding.OnProduced += OnProduced;

        _armySlotElement = new();
        Add(_armySlotElement);

        if (_productionBuilding.AvailableToBuyCount > 0)
            _armySlotElement.AddArmy(new(new(_productionBuilding.ArmyEntity, _productionBuilding.AvailableToBuyCount)));
    }

    void OnProduced(int count)
    {
        _armySlotElement.RemoveArmy();
        _armySlotElement.AddArmy(new(new(_productionBuilding.ArmyEntity, count)));
    }
}
