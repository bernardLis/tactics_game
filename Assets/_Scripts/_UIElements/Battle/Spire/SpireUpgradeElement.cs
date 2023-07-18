using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpireUpgradeElement : VisualElement
{
    const string _ussClassName = "spire-upgrade__";
    const string _ussMain = _ussClassName + "main";

    GameManager _gameManager;

    Storey _upgrade;

    PurchaseButton _purchaseButton;
    public SpireUpgradeElement(Storey upgrade)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.SpireUpgradeStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _upgrade = upgrade;
        AddToClassList(_ussMain);

        VisualElement container = new();
        Add(container);

        Label name = new(upgrade.name);
        container.Add(name);
        Label icon = new();
        icon.style.backgroundImage = new StyleBackground(upgrade.Icon);
        container.Add(icon);

        Label desc = new(upgrade.Description);
        Add(desc);

        _purchaseButton = new(upgrade.Cost, callback: Purchased, isPurchased: upgrade.IsPurchased);
        Add(_purchaseButton);
    }

    void Purchased()
    {
        _upgrade.Purchased();
    }
}
