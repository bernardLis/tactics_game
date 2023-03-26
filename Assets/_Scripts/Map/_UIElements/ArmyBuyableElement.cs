using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ArmyBuyableElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonMenuButton = "common__menu-button";

    const string _ussClassName = "army-buyable";
    const string _ussMain = _ussClassName + "__main";

    GameManager _gameManager;

    ProductionBuilding _building;

    ArmySlotElement _armySlotElement;
    SliderInt _slider;
    MyButton _buyButton;
    GoldElement _armyCost;

    public ArmyBuyableElement(ProductionBuilding building)
    {
        _gameManager = GameManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ArmyBuyableElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _building = building;
        _building.OnAvailableToBuyCountChanged += OnAvailableCountChanged;


        // slot + army (slot is locked) - TODO: not draggable
        _armySlotElement = new();
        Add(_armySlotElement);

        if (_building.AvailableToBuyCount > 0)
            _armySlotElement.AddArmy(new(_building.GetAvailableArmyGroup()));

        _slider = new(0, _building.AvailableToBuyCount);
        Add(_slider);

        _slider.RegisterValueChangedCallback(x =>
        {
            int cost = x.newValue * _building.PricePerEntity;
            _armyCost.ChangeAmount(cost);

            _buyButton.SetEnabled(true);
            if (cost > _gameManager.Gold)
                _buyButton.SetEnabled(false);
        });

        // buy button
        _buyButton = new("", _ussCommonMenuButton, BuyArmy);
        _armyCost = new(0);
        _buyButton.Add(_armyCost);
        Add(_buyButton);
    }

    void OnAvailableCountChanged(int count)
    {
        _armySlotElement.RemoveArmy();
        if (count > 0)
            _armySlotElement.AddArmy(new(_building.GetAvailableArmyGroup()));

        _slider.highValue = _building.AvailableToBuyCount;
    }

    void BuyArmy()
    {
        int cost = _slider.value * _building.PricePerEntity;

        _gameManager.ChangeGoldValue(-cost);

        ArmyGroup armyGroup = ScriptableObject.CreateInstance<ArmyGroup>();
        armyGroup.ArmyEntity = _building.ArmyEntity;
        armyGroup.EntityCount = _slider.value;

        _building.Sell(_slider.value);
    }
}
