using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemSlotVisual : VisualElement
{
    AudioManager _audioManager;

    public ItemVisual ItemVisual;
    public Character Character;

    public ItemSlotVisual(ItemVisual item = null)
    {
        _audioManager = AudioManager.Instance;
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

    void PlayClick()
    {
        _audioManager.PlaySFX("uiClick", Vector3.zero);
    }
}
