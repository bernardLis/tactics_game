using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class GlobalUpgradeVisual : VisualWithTooltip
{
    public GlobalUpgrade Upgrade;
    string _tooltipText;

    VisualElement _icon;
    Label _price;
    public GlobalUpgradeVisual(GlobalUpgrade upgrade, bool bought = false) : base()
    {
        Upgrade = upgrade;
        _tooltipText = upgrade.Tooltip;

        style.alignSelf = Align.FlexStart;

        _icon = new VisualElement();
        _icon.style.backgroundImage = upgrade.Sprite.texture;
        _icon.style.backgroundColor = Color.gray;
        _icon.style.width = 200;
        _icon.style.height = 200;

        _price = new Label($"Price: {upgrade.Price}");
        _price.AddToClassList("textSecondary");

        if (bought)
        {
            _icon.style.backgroundColor = Color.green;
            _price.text = "Bought!";
        }

        Add(_icon);
        Add(_price);
    }

    public void PurchaseUpgrade()
    {
        _icon.style.backgroundColor = Color.green;
        _price.text = "Bought!";
    }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, _tooltipText);
        base.DisplayTooltip();
    }

}
