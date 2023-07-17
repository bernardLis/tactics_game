using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PurchaseButton : MyButton
{
    const string _ussCommonRerollButton = "common__purchase-button";

    GoldElement _costGoldElement;

    int _cost;

    public PurchaseButton(int cost, string buttonText = "", string className = _ussCommonRerollButton, Action callback = null)
            : base(buttonText, className, callback)
    {
        _cost = cost;

        _costGoldElement = new(cost);
        Add(_costGoldElement);

        if (_gameManager.Gold < cost)
            SetEnabled(false);

        _gameManager.OnGoldChanged += UpdateButton;
        
        RegisterCallback<PointerUpEvent>((a) => _gameManager.ChangeGoldValue(-cost));
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
