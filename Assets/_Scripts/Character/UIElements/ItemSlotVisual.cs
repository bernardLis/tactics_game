using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class ItemSlotVisual : VisualElementWithSound
{

    public ItemVisual ItemVisual;
    public Character Character;

    bool _isHighlighted;

    public ItemSlotVisual(ItemVisual item = null) : base()
    {
        RegisterCallback<MouseEnterEvent>((evt) => PlayClick());

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
        PlayClick();
    }

    public void RemoveItem()
    {
        Clear();
        ItemVisual = null;
    }

    public async void Highlight()
    {
        _isHighlighted = true;

        while (_isHighlighted)
        {
            ToggleInClassList("itemSlotHighlighted");
            ToggleInClassList("itemSlot");
            await Task.Delay(500);
        }
    }

    public void ClearHighlight()
    {
        _isHighlighted = false;

        AddToClassList("itemSlot");
        RemoveFromClassList("itemSlotHighlighted");
    }
}
