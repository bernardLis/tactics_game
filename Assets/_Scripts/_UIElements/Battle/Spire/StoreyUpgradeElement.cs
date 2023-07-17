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
    const string _ussIcon = _ussClassName + "icon";

    GameManager _gameManager;

    public StoreyUpgrade StoreyUpgrade;

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

        Label title = new(storeyUpgrade.name);
        Add(title);

        Label icon = new();
        icon.AddToClassList(_ussIcon);
        icon.style.backgroundImage = new StyleBackground(storeyUpgrade.Icon);
        Add(icon);

        PurchaseButton purchaseButton = new(storeyUpgrade.Cost, callback: Purchase);
        Add(purchaseButton);
    }

    void Purchase()
    {
        OnPurchased?.Invoke(StoreyUpgrade);

        if (StoreyUpgrade.IsInfinite) return;
        SetEnabled(false);
    }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, new Label(StoreyUpgrade.Description));
        base.DisplayTooltip();
    }
}
