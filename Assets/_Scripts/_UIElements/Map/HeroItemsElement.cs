using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class HeroItemsElement : VisualElement
{
    Hero _hero;

    List<ItemSlot> _itemSlots = new();
    List<ItemElement> _itemElements = new();

    public HeroItemsElement(Hero hero)
    {
        _hero = hero;

        style.flexDirection = FlexDirection.Row;
        CreateItems();
    }

    void CreateItems()
    {
        int slotCount = 5;
        if (_hero.Items.Count > slotCount)
            slotCount = _hero.Items.Count;

        for (int i = 0; i < slotCount; i++)
        {
            ItemSlot itemSlot = new();
            itemSlot.OnItemAdded += OnItemAdded;
            itemSlot.OnItemRemoved += OnItemRemoved;

            itemSlot.Hero = _hero;
            _itemSlots.Add(itemSlot);
            Add(itemSlot);
        }

        for (int i = 0; i < _hero.Items.Count; i++)
        {
            ItemElement itemVisual = new ItemElement(_hero.Items[i]);
            _itemSlots[i].AddItemNoDelegates(itemVisual);
            _itemElements.Add(itemVisual);
        }

    }

    void OnItemAdded(ItemElement itemElement) { _hero.AddItem(itemElement.Item); }

    void OnItemRemoved(ItemElement itemElement) { _hero.RemoveItem(itemElement.Item); }
}
