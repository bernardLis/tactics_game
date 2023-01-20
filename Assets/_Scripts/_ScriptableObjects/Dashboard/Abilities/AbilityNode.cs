using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Ability Node")]
public class AbilityNode : BaseScriptableObject
{
    public bool IsPermaLocked;    
    public Sprite IconUnlocked;
    public Sprite IconLocked;
    public string Title;
    public string Description;
    public int SpiceCost;
    public int DaysToCraftAbility;
    public bool IsUnlocked;
    [HideInInspector] public bool IsOnCooldown;
    [HideInInspector] public int DaysOnCooldownRemaining;

    [Header("VFX")]
    public EffectHolder UnlockEffect;

    public EffectHolder AddedToCraftingEffect;
    public Vector3 AddedToCraftingEffectPosition;
    public Vector3 AddedToCraftingEffectScale;

    public EffectHolder AbilityCraftedEffect;
    public Vector3 AbilityCraftedEffectScale;

    [Header("Ability crafting values")]
    [Tooltip("Inclusive, inclusive")]
    public List<Ability> Abilities = new();

    GameManager _gameManager;

    public event Action OnCooldownChanged;
    public void Initialize()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
    }

    void OnDayPassed(int day)
    {
        if (!IsOnCooldown)
            return;

        DaysOnCooldownRemaining--;
        if (DaysOnCooldownRemaining <= 0)
            IsOnCooldown = false;
        OnCooldownChanged?.Invoke();
    }

    public bool Unlock()
    {
        if (IsPermaLocked)
            return false;
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
    }

    public Ability CreateAbility(int stars, string name)
    {
        Ability abilityToInstantiate = GetAbilityByStars(stars);

        Ability ability = Instantiate(abilityToInstantiate);
        ability.name = name;

        IsOnCooldown = true;
        DaysOnCooldownRemaining = DaysToCraftAbility;
        OnCooldownChanged?.Invoke();

        return ability;
    }

    public Ability GetAbilityByStars(int stars)
    {
        foreach (Ability a in Abilities)
            if (a.StarRank == stars)
                return a;

        Debug.LogError($"No ability with {stars} stars in {name}");
        return null;
    }

    public int GetSpiceCostByStars(int stars)
    {
        foreach (Ability a in Abilities)
            if (a.StarRank == stars)
                return a.SpiceCost;

        Debug.LogError($"No ability with {stars} stars in {name}");
        return -1;
    }

    List<Ability> GetOrderedAbilities() { return Abilities.OrderByDescending(a => a.StarRank).ToList(); }

    public Vector2Int GetStarRange()
    {
        List<Ability> orderedAbilities = GetOrderedAbilities();
        Ability aMin = orderedAbilities.Last();
        Ability aMax = orderedAbilities.First();
        return new Vector2Int(aMin.StarRank, aMax.StarRank);
    }

    public void LoadFromData(AbilityNodeData data)
    {
        IsUnlocked = data.IsUnlocked;
        IsOnCooldown = data.IsOnCooldown;
        DaysOnCooldownRemaining = data.DaysOnCooldownRemaining;
    }

    public AbilityNodeData SerializeSelf()
    {
        AbilityNodeData data = new();

        data.Id = Id;
        data.IsUnlocked = IsUnlocked;
        data.IsOnCooldown = IsOnCooldown;
        data.DaysOnCooldownRemaining = DaysOnCooldownRemaining;

        return data;
    }
}

[Serializable]
public struct AbilityNodeData
{
    public string Id;
    public bool IsUnlocked;
    public bool IsOnCooldown;
    public int DaysOnCooldownRemaining;
}