using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class HeroItemsElement : VisualElement
{
    MapHero _mapHero;

    List<ItemSlot> _itemSlots = new();
    List<ItemElement> _itemElements = new();

    public HeroItemsElement(MapHero mapHero)
    {
        _mapHero = mapHero;

        style.flexDirection = FlexDirection.Row;
        CreateItems();
    }

    void CreateItems()
    {
        int slotCount = 5;
        if (_mapHero.Character.Items.Count > slotCount)
            slotCount = _mapHero.Character.Items.Count;

        for (int i = 0; i < slotCount; i++)
        {
            ItemSlot itemSlot = new();
            itemSlot.OnItemAdded += OnItemAdded;
            itemSlot.OnItemRemoved += OnItemRemoved;

            itemSlot.Character = _mapHero.Character;
            _itemSlots.Add(itemSlot);
            Add(itemSlot);
        }

        for (int i = 0; i < _mapHero.Character.Items.Count; i++)
        {
            ItemElement itemVisual = new ItemElement(_mapHero.Character.Items[i]);
            _itemSlots[i].AddItemNoDelegates(itemVisual);
            _itemElements.Add(itemVisual);
        }

    }

    void OnItemAdded(ItemElement itemElement) { _mapHero.Character.AddItem(itemElement.Item); }

    void OnItemRemoved(ItemElement itemElement) { _mapHero.Character.RemoveItem(itemElement.Item); }


}
