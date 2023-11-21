using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GlobalUpgradeElement : ElementWithTooltip
{
    const string _ussCommonButtonBasic = "common__button-basic";
    const string _ussClassName = "global-upgrade__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTitle = _ussClassName + "title";
    const string _ussIcon = _ussClassName + "icon";

    GameManager _gameManager;

    public GlobalUpgrade GlobalUpgrade;

    Label _title;

    public GlobalUpgradeElement(GlobalUpgrade globalUpgrade)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.GlobalUpgradeStyles);
        if (ss != null)
            styleSheets.Add(ss);

        GlobalUpgrade = globalUpgrade;
        AddToClassList(_ussMain);
        AddToClassList(_ussCommonButtonBasic);

        _title = new(Helpers.ParseScriptableObjectName(globalUpgrade.name));
        _title.AddToClassList(_ussTitle);
        Add(_title);

        Label icon = new();
        icon.AddToClassList(_ussIcon);
        icon.style.backgroundImage = new StyleBackground(globalUpgrade.Icon);
        Add(icon);

        PurchaseButton purchaseButton = new(globalUpgrade.GetNextLevel().Cost, callback: Purchase);
        Add(purchaseButton);
    }

    public void UpdateTitle(string str)
    {
        _title.text = str;
    }

    void Purchase()
    {
        GlobalUpgrade.Purchased();

    }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, new Label(GlobalUpgrade.Description));
        base.DisplayTooltip();
    }
}
