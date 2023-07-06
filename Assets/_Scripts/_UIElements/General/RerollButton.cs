using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RerollButton : MyButton
{
    const string _ussCommonRerollButton = "common__reroll-button";
    const string _ussCommonRerollIcon = "common__reroll-icon";
    const string _ussCommonRerollIconHover = "common__reroll-icon-hover";

    GoldElement _rerollCost;

    public RerollButton(string buttonText = null, string className = _ussCommonRerollButton, Action callback = null)
            : base(buttonText, className, callback)
    {
        VisualElement rerollIcon = new();
        rerollIcon.AddToClassList(_ussCommonRerollIcon);
        Add(rerollIcon);

        RegisterCallback<PointerEnterEvent>(evt => rerollIcon.AddToClassList(_ussCommonRerollIconHover));
        RegisterCallback<PointerLeaveEvent>(evt => rerollIcon.RemoveFromClassList(_ussCommonRerollIconHover));

        _rerollCost = new(200);
        Add(_rerollCost);
    }
}
