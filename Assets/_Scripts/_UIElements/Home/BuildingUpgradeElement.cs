using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingUpgradeElement : ElementWithTooltip
{
    const string _ussCommonButtonBasic = "common__button-basic";
    const string _ussClassName = "building-upgrade__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTitle = _ussClassName + "title";
    const string _ussIcon = _ussClassName + "icon";

    GameManager _gameManager;

    public BuildingUpgrade BuildingUpgrade;

    Label _title;

    public event Action<BuildingUpgrade> OnPurchased;
    public BuildingUpgradeElement(BuildingUpgrade buildingUpgrade)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BuildingUpgradeStyles);
        if (ss != null)
            styleSheets.Add(ss);

        BuildingUpgrade = buildingUpgrade;
        AddToClassList(_ussMain);
        AddToClassList(_ussCommonButtonBasic);

        _title = new(Helpers.ParseScriptableObjectName(buildingUpgrade.name));
        _title.AddToClassList(_ussTitle);
        Add(_title);

        Label icon = new();
        icon.AddToClassList(_ussIcon);
        icon.style.backgroundImage = new StyleBackground(buildingUpgrade.Icon);
        Add(icon);

        PurchaseButton purchaseButton = new(buildingUpgrade.Cost, callback: Purchase,
                isInfinite: buildingUpgrade.IsInfinite, isPurchased: buildingUpgrade.IsPurchased);
        Add(purchaseButton);
    }

    public void UpdateTitle(string str)
    {
        _title.text = str;
    }

    void Purchase()
    {
        OnPurchased?.Invoke(BuildingUpgrade);

        if (BuildingUpgrade.IsInfinite) return;
        BuildingUpgrade.IsPurchased = true;
        SetEnabled(false);
    }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, new Label(BuildingUpgrade.Description));
        base.DisplayTooltip();
    }
}
