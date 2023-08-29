using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GoldElement : ChangingValueElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "gold-element__";
    const string _ussMain = _ussClassName + "main";
    const string _ussIcon = _ussClassName + "icon";
    const string _ussValue = _ussClassName + "value";

    GameManager _gameManager;
    VisualElement _icon;

    public GoldElement(int amount)
    {
        _gameManager = GameManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.GoldElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Amount = 0;

        AddToClassList(_ussMain);

        _icon = new();
        _icon.AddToClassList(_ussIcon);
        _icon.style.backgroundImage = new StyleBackground(_gameManager.GameDatabase.GetCoinSprite(amount));
        Add(_icon);

        _text = new();
        _text.AddToClassList(_ussCommonTextPrimary);
        _text.AddToClassList(_ussValue);
        _text.text = Amount.ToString();
        Add(_text);

        ChangeAmount(amount);
    }

    protected override void NumberAnimation()
    {
        base.NumberAnimation();
        _icon.style.backgroundImage = new StyleBackground(_gameManager.GameDatabase.GetCoinSprite(_currentlyDisplayedAmount));
    }
}
