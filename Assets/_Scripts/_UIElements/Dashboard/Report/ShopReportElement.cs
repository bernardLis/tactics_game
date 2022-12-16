using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopReportElement : ReportElement
{

    VisualElement _itemsContainer;

    public ShopReportElement(VisualElement parent, Report report) : base(parent, report)
    {
        AddHeader("Shop", Color.red);

        AddShopItems();

    }

    void AddShopItems()
    {
        _itemsContainer = new();
        // HERE: different way to choose items
        foreach (var item in _gameManager.ShopItems)
        {
            // so here I want 3 item slots that are filled with items 
            VisualElement container = new();
            container.AddToClassList("shopItem");

            ItemElement itemVisual = new(item);
            ItemSlot itemSlot = new();
            itemSlot.AddItem(itemVisual);

            //https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-Events-Handling.html
            //itemVisual.RegisterCallback<PointerDownEvent>(OnShopItemPointerDown);

            container.Add(itemSlot);
            container.Add(new GoldElement(item.Price));

            _itemsContainer.Add(container);
        }

    }
}
