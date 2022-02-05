using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "ScriptableObject/Inventory/Item")]
public class Item : BaseScriptableObject
{
    public string iDescription;
    public Sprite icon = null;
    public int amount = 1;
    public int sellPrice;
    public int stackSize = 5;
    public bool questItem;
    public Dimensions slotDimension;
    public Ability ability;

    public virtual void PickUp()
    {
        // TODO: maybe a bad idea? 
        InventoryManager.instance.Add(this);
    }

    public virtual void Use()
    {
        // meant to be overwritten
    }


}

[Serializable]
public struct Dimensions
{
    public int height;
    public int width;
}
