using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class ItemSlot : ElementWithSound
{

    public ItemElement ItemVisual;
    public Character Character;

    bool _isHighlighted;

    public ItemSlot(ItemElement item = null) : base()
    {
        RegisterCallback<MouseEnterEvent>((evt) => PlayClick());

        AddToClassList("itemSlot");

        if (item == null)
            return;

        ItemVisual = item;
        Add(item);
    }

    public void AddItem(ItemElement item)
    {
        ItemVisual = item;
        Add(item);
        PlayClick();
    }

    public void RemoveItem()
    {
        Clear();
        ItemVisual = null;
    }
}
