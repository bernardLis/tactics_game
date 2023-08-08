using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PurchaseButton : MyButton
{
    const string _ussCommonPurchaseButton = "common__purchase-button";
    const string _ussCommonPurchased = "common__purchased";

    GoldElement _costGoldElement;

    int _cost;
    bool _isInfinite;

    public PurchaseButton(int cost, bool isInfinite = false, bool isPurchased = false,
            string buttonText = "", string className = _ussCommonPurchaseButton, Action callback = null)
            : base(buttonText, className, callback)
    {
        _cost = cost;
        _isInfinite = isInfinite;

        if (isPurchased)
        {
            SetEnabled(false);
            AddToClassList(_ussCommonPurchased);
            return;
        }

        _costGoldElement = new(cost);
        Add(_costGoldElement);

        if (_gameManager.Gold < cost)
            SetEnabled(false);

        _gameManager.OnGoldChanged += UpdateButton;
        clicked += Buy;
    }

    public void UpdateCost(int cost)
    {
        _cost = cost;
        _costGoldElement.ChangeAmount(cost);
        if (_gameManager.Gold < cost)
            SetEnabled(false);
    }
    
    public void NoMoreInfinity()
    {
        _isInfinite = false;
    }

    void Buy()
    {
        if (_gameManager.Gold < _cost)
            return;

        _gameManager.ChangeGoldValue(-_cost);

        if (_isInfinite) return;

        _gameManager.OnGoldChanged -= UpdateButton;
        AddToClassList(_ussCommonPurchased);
        SetEnabled(false);
        Remove(_costGoldElement);
    }

    void UpdateButton(int gold)
    {
        if (gold < _cost)
        {
            SetEnabled(false);
            return;
        }
        SetEnabled(true);
    }
}
