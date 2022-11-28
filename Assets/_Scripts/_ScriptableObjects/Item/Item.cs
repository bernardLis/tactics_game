using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Items/Item")]
public class Item : BaseScriptableObject
{
    public string ReferenceID;
    public string ItemName;
    public Sprite Icon;
    public StatType InfluencedStat;
    public ItemRaririty Raririty;
    public int Value;
    public int Price;
    public Status Status;
    public string TooltipText;

    public int GetSellValue() { return Mathf.FloorToInt(Price * 0.5f); }

    public virtual void Initialize(CharacterStats stats) { }
}
