using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class GlobalUpgradeVisual : VisualWithTooltip
{
    GlobalUpgrade _globalUpgrade;
    string _tooltipText;
    public GlobalUpgradeVisual(GlobalUpgrade upgrade, bool bought = false) : base()
    {
        _globalUpgrade = upgrade;
        _tooltipText = upgrade.Tooltip;

        style.alignSelf = Align.FlexStart;

        VisualElement icon = new VisualElement();
        icon.style.backgroundImage = upgrade.Sprite.texture;
        icon.style.width = 200;
        icon.style.height = 200;

        Label price = new Label($"Price: {upgrade.Price}");
        price.AddToClassList("textSecondary");

        if (bought)
        {
            icon.style.color = Color.green;
            price.text = "Bought!";
        }

        Add(icon);
        Add(price);
    }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, _tooltipText);
        base.DisplayTooltip();
    }

}
