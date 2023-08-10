using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameDatabase : BaseScriptableObject
{
    [Header("Battle")]
    [SerializeField] BattleModifier[] BattleModifiers;
    public List<BattleModifier> GetAllBattleModifiers() { return new(BattleModifiers); }
    public BattleModifier GetRandomBattleModifier() { return BattleModifiers[Random.Range(0, BattleModifiers.Length)]; }

    [Header("Base")]
    public List<Storey> AllBaseUpgrades = new();
    public GameObject BaseGameObject;

    [Header("Shaders")]
    public Shader DissolveShader;
    public Shader GrayScaleShader;

    [Header("General")]
    [SerializeField] ColorVariable[] _colors;
    public ColorVariable GetColorByName(string name) { return _colors.FirstOrDefault(c => c.name == name); }

    [SerializeField] Sprite[] CoinSprites;
    public Sprite[] LevelUpAnimationSprites;
    public Sprite[] TroopsElementAnimationSprites;
    [SerializeField] SpiceAnimations[] SpiceAnimationSprites;
    [Serializable] public class SpiceAnimations { public Sprite[] sprites; }

    public Sprite GetCoinSprite(int amount)
    {
        int index = 0;
        // TODO: something smarter
        if (amount >= 0 && amount <= 100)
            index = 0;
        if (amount >= 101 && amount <= 500)
            index = 1;
        if (amount >= 501 && amount <= 1000)
            index = 2;
        if (amount >= 1001 && amount <= 3000)
            index = 3;
        if (amount >= 3001)
            index = 4;

        return CoinSprites[index];
    }

    public Sprite[] GetSpiceSprites(int amount)
    {
        int index = 0;
        // TODO: something smarter
        if (amount >= 0 && amount <= 20)
            index = 0;
        if (amount >= 21 && amount <= 50)
            index = 1;
        if (amount >= 51 && amount <= 100)
            index = 2;
        if (amount >= 101 && amount <= 200)
            index = 3;
        if (amount >= 201 && amount <= 300)
            index = 4;
        if (amount >= 301)
            index = 5;

        return SpiceAnimationSprites[index].sprites;
    }
}

public enum ItemRarity { Common, Uncommon, Rare, Epic }
public enum StatType { Power, Health, Mana, Armor, Speed }

public enum ElementName { Fire, Water, Wind, Earth }

[System.Serializable]
public struct StatIcon
{
    public string StatName;
    public Sprite Sprite;
}



