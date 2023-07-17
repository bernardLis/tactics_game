using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradeLevelElement : ElementWithTooltip
{
    StoreyUpgrade _upgradeLevel;

    public event Action OnPurchased;
    public UpgradeLevelElement(StoreyUpgrade upgradeLevel)
    {
        _upgradeLevel = upgradeLevel;
        
        Label title = new(upgradeLevel.name);
        Add(title);

        Label icon = new();
        icon.style.backgroundImage = new StyleBackground(upgradeLevel.Icon);
        Add(icon);

        PurchaseButton purchaseButton = new(upgradeLevel.Cost, callback: Purchase);
        Add(purchaseButton);
    }

    void Purchase()
    {
        OnPurchased?.Invoke();

        if (_upgradeLevel.IsInfinite) return;
        SetEnabled(false);
    }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, new Label(_upgradeLevel.Description));
        base.DisplayTooltip();
    }
}
