using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseButton : MyButton
{
    const string _ussCommonRerollButton = "common__reroll-button";


    GoldElement _costGoldElement;

    public PurchaseButton(int cost, string buttonText = null, string className = _ussCommonRerollButton, Action callback = null)
            : base(buttonText, className, callback)
    {
        _costGoldElement = new(cost);
        Add(_costGoldElement);

        if(_gameManager.Gold < cost)
            SetEnabled(false);
    }
}
