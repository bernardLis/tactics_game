using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Ability Node")]
public class AbilityNode : BaseScriptableObject
{
    public Sprite IconUnlocked;
    public Sprite IconLocked;
    public string Title;
    public string Description;
    public int SpiceCost;
    public bool IsUnlocked;

    // HERE: possibly range of range of ability created through this node 
    // HERE: possibly range of damage of ability created through this node 
    // HERE: base ability scriptable object created from this  

    public bool Unlock()
    {
        if (IsUnlocked)
            return false;
        if (!PayForUnlocking())
            return false;

        IsUnlocked = true;
        return true;
    }

    bool PayForUnlocking()
    {
        GameManager gameManager = GameManager.Instance;

        if (gameManager.Spice < SpiceCost)
            return false;

        gameManager.ChangeSpiceValue(-SpiceCost);
        return true;
        // pay return true
    }

    public void LoadFromData(AbilityNodeData data)
    {
        IsUnlocked = data.IsUnlocked;
    }

    public AbilityNodeData SerializeSelf()
    {
        AbilityNodeData data = new();

        data.Id = Id;
        data.IsUnlocked = IsUnlocked;

        return data;
    }
}


[Serializable]
public struct AbilityNodeData
{
    public string Id;
    public bool IsUnlocked;
}
