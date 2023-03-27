using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

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
        _productionBuilding.OnBuilt += AddArmyBuyableElementWithEffect;

        if (!_productionBuilding.IsBuilt)
            return;

        AddArmyBuyableElement();
    }
    
    void AddArmyBuyableElement()
    {
        ArmyBuyableElement = new ArmyBuyableElement(_productionBuilding);
        Add(ArmyBuyableElement);
    }

    void AddArmyBuyableElementWithEffect()
    {
        AddArmyBuyableElement();

        ArmyBuyableElement.transform.scale = Vector3.zero;
        DOTween.To(x => ArmyBuyableElement.transform.scale = x * Vector3.one, 0, 1, 1f)
                .SetEase(Ease.InCirc)
                .SetDelay(1f);

    }
}
