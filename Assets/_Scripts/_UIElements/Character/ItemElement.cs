using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class ItemElement : ElementWithTooltip
{
    string _tooltipText;
    public Item Item;
    public bool IsShop;
    ShopReportElement _shopReportElement;
    public ItemElement(Item item, ShopReportElement shopReportElement = null) : base()
    {
        Item = item;
        
        if (shopReportElement != null)
        {
            _shopReportElement = shopReportElement;
            IsShop = true;
        }

        style.backgroundImage = item.Icon.texture;
        style.width = 75;
        style.height = 75;
        if (item.TooltipText.Length == 0)
            _tooltipText = item.InfluencedStat.ToString() + " +" + item.Value.ToString();
        else
            _tooltipText = item.TooltipText;
    }

    public void ItemBought()
    {
        _shopReportElement.ItemBought(Item);
        IsShop = false;
    }

    protected override void DisplayTooltip()
    {
        VisualElement tooltip = new();

        Label name = new(Item.ItemName);
        Label tooltipText = new(_tooltipText);
        Label rarity = new($"Rarity: {Item.Rarity}");
        rarity.style.color = Helpers.GetColor(Item.Rarity.ToString());
        Label value = new($"Price: {Item.Price}");

        tooltip.Add(name);
        tooltip.Add(tooltipText);
        tooltip.Add(rarity);
        tooltip.Add(value);

        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }

}
