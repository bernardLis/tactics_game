using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "ScriptableObject/Inventory/Item")]
public class Item : BaseScriptableObject
{
    public string Description;
    public Sprite Icon = null;
    public int Amount = 1;
    public int SellPrice;
    public int StackSize = 5;
    public bool QuestItem;
    public Dimensions SlotDimension;
    public Ability Ability;

    public virtual void PickUp()
    {
        // TODO: maybe a bad idea? 
        InventoryManager.Instance.Add(this);
    }

    public virtual void Use()
    {
        // meant to be overwritten
    }


}

[Serializable]
public struct Dimensions
{
    public int Height;
    public int Width;
}
