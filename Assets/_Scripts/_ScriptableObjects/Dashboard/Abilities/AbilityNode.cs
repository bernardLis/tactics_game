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
    public EffectHolder UnlockEffect;

    [Header("Ability crafting values")]
    public Vector2Int RangeMinMax;
    public Vector2Int DamageMinMax;
    public Vector2Int AOEMinMax;

    public int ManaCostRangePoint;
    public int ManaCostDamagePoint;
    public int ManaCostAOEPoint;
    public Vector2Int ManaCostMinMax;

    public Status Status;
    public int ManaCostStatus;

    public Ability AbilityTemplate;

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

    public int CalculateManaCost(int range, int damage, int aoe, bool status)
    {
        int total = 0;
        total += range * ManaCostRangePoint;
        total += Mathf.FloorToInt(damage * 0.1f * ManaCostDamagePoint);
        total += aoe * ManaCostAOEPoint;
        if (status)
            total += ManaCostStatus;

        return total;
    }

    public AbilityCraftingValidity CheckAbilityValidity(int range, int damage, int aoe, bool status)
    {
        int manaCost = CalculateManaCost(range, damage, aoe, status);
        if (manaCost > ManaCostMinMax.y)
            return new AbilityCraftingValidity(false, manaCost, "Max mana cost exceeded.");

        if (range > RangeMinMax.y || damage > DamageMinMax.y || aoe > AOEMinMax.y)
            return new AbilityCraftingValidity(false, manaCost, "Max range exceeded.");

        if (range < RangeMinMax.x || damage < DamageMinMax.x || aoe < AOEMinMax.x)
            return new AbilityCraftingValidity(false, manaCost, "That's too low.");

        AbilityCraftingValidity validity = new(true, manaCost, null);
        return validity;
    }

    public Ability CreateAbility(string name, int range, int damage, int aoe, bool status)
    {
        int manaCost = CalculateManaCost(range, damage, aoe, status);

        Ability ability = Instantiate(AbilityTemplate);
        ability.name = name;
        ability.Range = range;
        ability.BasePower = damage;
        ability.AreaOfEffect = aoe;
        ability.ManaCost = manaCost;
        if (!status)         // HERE: hmm... something smarter...
            ability.Status = null;

        return ability;
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

[Serializable]
public struct AbilityCraftingValidity
{
    public AbilityCraftingValidity(bool isValid, int manaCost, string message)
    {
        IsValid = isValid;
        ManaCost = manaCost;
        Message = message;
    }
    public bool IsValid;
    public int ManaCost;
    public string Message;
}
