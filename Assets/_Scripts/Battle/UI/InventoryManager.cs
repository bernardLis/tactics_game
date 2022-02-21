using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


// https://www.youtube.com/watch?v=NJLOnRzTPFo&list=PLAE7FECFFFCBE1A54&index=18
public class ItemChangedEventArgs : EventArgs
{
    public Item item { get; private set; }
    public bool added;
    public ItemChangedEventArgs(Item _item, bool _added)
    {
        item = _item;
        added = _added;
    }
}

public class InventoryManager : MonoBehaviour
{
    public List<Item> items = new();

    public event EventHandler<ItemChangedEventArgs> OnItemChanged;

    #region Singleton
    public static InventoryManager instance;
    void Awake()
    {
        // singleton
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of InventoryManager found");
            return;
        }
        instance = this;

    }
    #endregion

    public void Add(Item item)
    {
        items.Add(item);

        GameUI.instance.DisplayLogText("+ 1 " + item.name);
        OnItemChanged?.Invoke(this, new ItemChangedEventArgs(item, true));

    }

    public void Remove(Item item)
    {
        items.Remove(item);

        OnItemChanged?.Invoke(this, new ItemChangedEventArgs(item, false));
    }
}
