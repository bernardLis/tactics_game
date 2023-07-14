using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseUpgradeElement : VisualElement
{

    BaseUpgrade _upgrade;

    PurchaseButton _purchaseButton;
    public BaseUpgradeElement(BaseUpgrade upgrade)
    {
        _upgrade = upgrade;
        
        style.flexDirection = FlexDirection.Row;

        VisualElement container = new();
        Add(container);

        Label name = new(upgrade.name);
        container.Add(name);
        Label icon = new();
        icon.style.backgroundImage = new StyleBackground(upgrade.Icon);
        container.Add(icon);

        Label desc = new(upgrade.Description);
        Add(desc);

        if (upgrade.IsPurchased)
        {
            Label p = new("Purchased");
            Add(p);
            return;
        }

        _purchaseButton = new(upgrade.Cost, callback: Purchased);
        Add(_purchaseButton);
    }

    void Purchased()
    {
        _upgrade.Purchased();

        _purchaseButton.RemoveFromHierarchy();
        Label p = new("Purchased");
        Add(p);
    }
}
