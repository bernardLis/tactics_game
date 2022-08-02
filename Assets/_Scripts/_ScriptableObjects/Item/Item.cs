using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Item")]
public class Item : BaseScriptableObject
{
    public string ReferenceID;
    public Sprite Icon;
    public StatType InfluencedStat;
    public int Value;
    public int Price;
}
