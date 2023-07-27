using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StoreyUpgradeElement : ElementWithTooltip
{
    const string _ussCommonButtonBasic = "common__button-basic";
    const string _ussClassName = "storey-upgrade__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTitle = _ussClassName + "title";
    const string _ussIcon = _ussClassName + "icon";

    GameManager _gameManager;

    public StoreyUpgrade StoreyUpgrade;

    Label _title;

    public event Action<StoreyUpgrade> OnPurchased;
    public StoreyUpgradeElement(StoreyUpgrade storeyUpgrade)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.StoreyUpgradeStyles);
        if (ss != null)
            styleSheets.Add(ss);

        StoreyUpgrade = storeyUpgrade;
        AddToClassList(_ussMain);
        AddToClassList(_ussCommonButtonBasic);

        _title = new(Helpers.ParseScriptableObjectName(storeyUpgrade.name));
        _title.AddToClassList(_ussTitle);
        Add(_title);

        Label icon = new();
        icon.AddToClassList(_ussIcon);
        icon.style.backgroundImage = new StyleBackground(storeyUpgrade.Icon);
        Add(icon);

        PurchaseButton purchaseButton = new(storeyUpgrade.Cost, callback: Purchase,
                isInfinite: storeyUpgrade.IsInfinite, isPurchased: storeyUpgrade.IsPurchased);
        Add(purchaseButton);
    }

    public void UpdateTitle(string str)
    {
        _title.text = str;
    }

    void Purchase()
    {
        OnPurchased?.Invoke(StoreyUpgrade);

        if (StoreyUpgrade.IsInfinite) return;
        StoreyUpgrade.IsPurchased = true;
        SetEnabled(false);
    }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, new Label(StoreyUpgrade.Description));
        base.DisplayTooltip();
    }
}
