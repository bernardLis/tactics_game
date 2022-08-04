using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemSlotVisual : VisualElement
{
    public ItemVisual ItemVisual;
    public Character Character;
    
    public ItemSlotVisual(ItemVisual item = null)
    {
        AddToClassList("itemSlot");

        if (item == null)
            return;

        ItemVisual = item;
        Add(item);
    }

    public void AddItem(ItemVisual item)
    {
        ItemVisual = item;
        Add(item);
    }
}
