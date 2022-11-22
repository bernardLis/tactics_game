using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Ability Node")]
public class AbilityNode : BaseScriptableObject
{
    public Sprite Icon;
    public string Title;
    public string Description;
    public AbilityNodeUnlockCost AbilityNodeUnlockCost;
    public bool IsUnlocked;

    // HERE: possibly range of range of ability created through this node 
    // HERE: possibly range of damage of ability created through this node 
    // HERE: base ability scriptable object created from this  

    public void Unlock()
    {
        IsUnlocked = true;
    }
}


[Serializable]
public struct AbilityNodeUnlockCost
{
    public int YellowSpiceCost;
    public int BlueSpiceCost;
    public int RedSpiceCost;
}