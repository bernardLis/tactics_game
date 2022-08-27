using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemVisual : VisualWithTooltip
{
    string _tooltipText;
    public Item Item;
    public ItemVisual(Item item) : base()
    {
        Item = item;

        style.backgroundImage = item.Icon.texture;
        style.unityBackgroundImageTintColor = Helpers.GetColor(item.Raririty.ToString());
        style.width = 75;
        style.height = 75;
        if (item.TooltipText.Length == 0)
            _tooltipText = item.InfluencedStat.ToString() + " +" + item.Value.ToString();
        else
            _tooltipText = item.TooltipText;

    }

    protected override void DisplayTooltip()
    {
        Label tooltip = new(_tooltipText);
        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }

}
