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
    public int DaysToCraftAbility;
    public bool IsUnlocked;

    [Header("VFX")]
    public EffectHolder UnlockEffect;

    public EffectHolder AddedToCraftingEffect;
    public Vector3 AddedToCraftingEffectPosition;
    public Vector3 AddedToCraftingEffectScale;

    public EffectHolder AbilityCraftedEffect;
    public Vector3 AbilityCraftedEffectScale;


    [Header("Ability crafting values")]
    [Tooltip("Inclusive, inclusive")]
    public Vector2Int StarRange;
    public AbilityNodeTemplate[] AbilityNodeTemplates;


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
    public Ability CreateAbility(int stars, string name)
    {
        Ability abilityToInstantiate = GetAbilityByStars(stars);

        Ability ability = Instantiate(abilityToInstantiate);
        ability.name = name;

        return ability;
    }

    public Ability GetAbilityByStars(int stars)
    {
        foreach (AbilityNodeTemplate t in AbilityNodeTemplates)
            if (t.Stars == stars)
                return t.Ability;

        Debug.LogError($"No ability with {stars} stars in {name}");
        return null;
    }

    public int GetSpiceCostByStars(int stars)
    {
        foreach (AbilityNodeTemplate t in AbilityNodeTemplates)
            if (t.Stars == stars)
                return t.CraftCost;

        Debug.LogError($"No ability with {stars} stars in {name}");
        return -1;

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
public struct AbilityNodeTemplate
{
    public int Stars;
    public Ability Ability;
    public int CraftCost;
}
