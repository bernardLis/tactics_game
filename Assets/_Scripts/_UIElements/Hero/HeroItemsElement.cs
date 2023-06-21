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
        style.justifyContent = Justify.Center;
        CreateItems();
    }

    void CreateItems()
    {
        ScrollView scrollView = new ScrollView();
        scrollView.contentContainer.style.flexDirection = FlexDirection.Row;
        Add(scrollView);

        for (int i = 0; i < _hero.Items.Count; i++)
        {
            ItemSlot itemSlot = new();
            itemSlot.OnItemAdded += OnItemAdded;
            itemSlot.OnItemRemoved += OnItemRemoved;

            itemSlot.Hero = _hero;
            _itemSlots.Add(itemSlot);
            scrollView.Add(itemSlot);
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