using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpireUpgradeElement : ElementWithTooltip
{
    const string _ussCommonButtonBasic = "common__button-basic";

    const string _ussClassName = "spire-upgrade__";
    const string _ussMain = _ussClassName + "main";
    const string _ussIcon = _ussClassName + "icon";

    GameManager _gameManager;

    Storey _storey;

    PurchaseButton _purchaseButton;

    public event Action OnPurchased;
    public SpireUpgradeElement(Storey storey)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.SpireUpgradeStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _storey = storey;

        AddToClassList(_ussMain);

        Label name = new(Helpers.ParseScriptableObjectCloneName(storey.name));
        Add(name);

        Label icon = new();
        icon.style.backgroundImage = new StyleBackground(storey.Icon);
        icon.AddToClassList(_ussIcon);
        Add(icon);

        _purchaseButton = new(storey.Cost, callback: Purchased, isPurchased: storey.IsPurchased);
        Add(_purchaseButton);
    }

    void Purchased()
    {
        _storey.Purchased();
        OnPurchased?.Invoke();
    }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, new Label(_storey.Description));
        base.DisplayTooltip();
    }


}
