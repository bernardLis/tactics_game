using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameDatabase : BaseScriptableObject
{
    [Header("Buildings")]
    [SerializeField] BuildingProduction[] Buildings;
    public BuildingProduction GetBuildingByName(string name) { return Buildings.FirstOrDefault(b => b.name == name); }

    [Header("Shaders")]
    public Shader LitShader;
    public Shader ParticlesUnlitShader;
    public Shader DissolveShader;
    public Shader GrayScaleShader;
    public Shader SepiaToneShader;

    public List<Shader> KeepShadersMaterials = new();


    [Header("General")]
    public RewardIcon[] RewardIcons;

    [SerializeField] ColorVariable[] _colors;
    public ColorVariable GetColorByName(string name) { return _colors.FirstOrDefault(c => c.name == name); }

    [SerializeField] Sprite[] CoinSprites;
    public Sprite[] LevelUpAnimationSprites;
    public Sprite[] TroopsElementAnimationSprites;
    [SerializeField] SpiceAnimations[] SpiceAnimationSprites;
    [Serializable] public class SpiceAnimations { public Sprite[] sprites; }
    [SerializeField] OpponentGroupIcon[] OpponentGroupIcons;
    public Sprite GetOpponentGroupIcon(Element element, bool hasCreatures)
    {
        OpponentGroupIcon icon = OpponentGroupIcons.FirstOrDefault(i => i.Element == element);
        return hasCreatures ? icon.SpriteCreature : icon.SpriteMinion;
    }

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
public enum StatType { Power, Health, Armor, Speed, AttackRange, AttackCooldown, Pull }

public enum ElementName { Fire, Water, Wind, Earth }

public enum MinionSpawningPattern { SurroundMiddle, Random, FewGroups, OneGroup, Grid }

[System.Serializable]
public struct RewardIcon
{
    public string Text;
    public Sprite Sprite;
}

[System.Serializable]
public struct StatBasics
{
    public StatType StatType;
    public Sprite Sprite;
    public string Description;
}

[System.Serializable]
public struct OpponentGroupIcon
{
    public Element Element;
    public Sprite SpriteCreature;
    public Sprite SpriteMinion;
}




