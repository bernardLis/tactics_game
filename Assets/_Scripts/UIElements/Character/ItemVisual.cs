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
        style.width = 75;
        style.height = 75;
        if (item.TooltipText.Length == 0)
            _tooltipText = item.InfluencedStat.ToString() + " +" + item.Value.ToString();
        else
            _tooltipText = item.TooltipText;
    }

    protected override void DisplayTooltip()
    {
        VisualElement tooltip = new();

        Label name = new(Item.ItemName);
        Label tooltipText = new(_tooltipText);
        Label rarity = new($"Rarity: {Item.Raririty}");
        rarity.style.color = Helpers.GetColor(Item.Raririty.ToString());
        Label value = new($"Price: {Item.Price}");

        tooltip.Add(name);
        tooltip.Add(tooltipText);
        tooltip.Add(rarity);
        tooltip.Add(value);

        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }

}
