using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class ItemSlot : ElementWithSound
{

    public ItemElement ItemElement;
    public Character Character;

    bool _isHighlighted;

    public event Action<ItemElement> OnItemAdded;
    public event Action<ItemElement> OnItemRemoved;
    public ItemSlot(ItemElement item = null) : base()
    {
        RegisterCallback<MouseEnterEvent>((evt) => PlayClick());
        AddToClassList("itemSlot");

        if (item == null)
            return;

        ItemElement = item;
        Add(item);
    }

    public void AddItem(ItemElement item)
    {
        AddItemNoDelegates(item);
        PlayClick();
        OnItemAdded?.Invoke(item);
    }

    public void AddItemNoDelegates(ItemElement item)
    {
        ItemElement = item;
        item.style.position = Position.Relative;
        Add(item);
    }

    public void RemoveItem()
    {
        OnItemRemoved?.Invoke(ItemElement);
        Clear();
        ItemElement = null;
    }
}
