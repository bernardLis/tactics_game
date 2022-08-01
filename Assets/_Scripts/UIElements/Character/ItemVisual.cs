using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemVisual : VisualWithTooltip
{
    string _tooltipText;
    public ItemVisual(Item item) : base()
    {
        style.backgroundImage = item.Icon.texture;
        style.width = 50;
        style.height = 50;
        _tooltipText = item.InfluencedStat.ToString() + " +" + item.Value.ToString();
    }


    protected override void DisplayTooltip()
    {
        _tooltip = new(this, _tooltipText);
        base.DisplayTooltip();
    }


}
