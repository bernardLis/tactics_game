using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : BaseScriptableObject
{
    public float LootChance;
    public ColorVariable PrefabColor;
    public ColorVariable DisplayColor;

    public GameObject Prefab;
    public GameObject Effect;
    public GameObject ClickEffect;

    public Sound DropSound;
    public Sound CollectSound;

    protected GameManager _gameManager;

    public void Initialize()
    {
        _gameManager = GameManager.Instance;
        SelectPrize();
        //   Debug.Log($"Initializing Loot {name}, roll: {roll}, gold: {Gold}, spice: {Spice}, item: {Item}");
    }

    protected virtual void SelectPrize()
    {
        // Meant to be overwritten
    }

    public virtual void Collect()
    {
        // Meant to be overwritten

    }

    public virtual string GetDisplayText()
    {
        // Meant to be overwritten
        return "";
    }

    public virtual Color GetDisplayColor()
    {
        // Meant to be overwritten
        return Color.white;
    }
}
