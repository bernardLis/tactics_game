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

    const string _ussClassName = "item-slot";
    const string _ussMain = _ussClassName + "__main";

    public event Action<ItemElement> OnItemAdded;
    public event Action<ItemElement> OnItemRemoved;
    public ItemSlot(ItemElement item = null) : base()
    {
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ItemSlotStyles);
        if (ss != null)
            styleSheets.Add(ss);

        RegisterCallback<MouseEnterEvent>((evt) => PlayClick());
        AddToClassList(_ussMain);

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
