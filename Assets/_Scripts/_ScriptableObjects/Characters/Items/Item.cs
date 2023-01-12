using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Items/Item")]
public class Item : BaseScriptableObject
{
    public string ItemName;
    public Sprite Icon;
    public StatType InfluencedStat;
    public ItemRarity Rarity;
    public int Value;
    public int Price;
    public Status Status;
    public string TooltipText;

    public int GetSellValue() { return Mathf.FloorToInt(Price * 0.5f); }

    public virtual void Initialize(CharacterStats stats) { }

    public void LoadFromData(ItemData data)
    {
        GameDatabase db = GameManager.Instance.GameDatabase;
        Item i = db.GetItemById(data.ItemId);
        Id = data.ItemId;
        ItemName = i.ItemName;
        Icon = i.Icon;
        InfluencedStat = i.InfluencedStat;
        Rarity = i.Rarity;
        Value = i.Value;
        Price = i.Price;
        Status = i.Status;
        TooltipText = i.TooltipText;
    }

    public ItemData SerializeSelf()
    {
        ItemData itemData = new();
        itemData.ItemId = Id;

        return itemData;
    }
}

[Serializable]
public struct ItemData
{
    public string ItemId;
}

