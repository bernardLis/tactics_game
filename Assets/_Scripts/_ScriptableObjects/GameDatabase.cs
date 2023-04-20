using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameDatabase : BaseScriptableObject
{
    [Header("Cutscenes")]
    [SerializeField] Cutscene[] Cutscenes;
    public Cutscene[] GetAllCutscenes() { return Cutscenes; }


    [Header("Dashboard")]
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


    public Sprite GoldSprite;
    public Sprite SpiceSprite;

    public List<ArmyEntity> AllArmyEntities = new();
    public ArmyEntity GetArmyEntityById(string id) { return AllArmyEntities.FirstOrDefault(x => x.Id == id); }

    public List<ArmyEntity> AllEnemyArmyEntities = new();
    public ArmyEntity GetEnemyArmyEntityById(string id) { return AllEnemyArmyEntities.FirstOrDefault(x => x.Id == id); }
    public ArmyEntity GetRandomEnemyArmyEntity() { return AllEnemyArmyEntities[Random.Range(0, AllEnemyArmyEntities.Count)]; }

    public List<ArmyGroup> BasicArmy = new();

    public List<Building> Buildings = new();
    public Building GetBuildingById(string id) { return Buildings.FirstOrDefault(x => x.Id == id); }

    public List<Castle> Castles = new();
    public Castle GetCastleById(string id) { return Castles.FirstOrDefault(x => x.Id == id); }

    public List<Map> Maps = new();
    public Map GetMapById(string id) { return Maps.FirstOrDefault(x => x.Id == id); }
}

public enum ItemRarity { Common, Uncommon, Rare, Epic }
public enum StatType { Power, Health, Mana, Armor, Speed }

public enum QuestState { Pending, Delegated, Finished, Expired, RewardCollected }
public enum DashboardBuildingType { Desk, Camp, Abilities }
public enum ElementName { Fire, Water, Wind, Earth }

[System.Serializable]
public struct StatIcon
{
    public string StatName;
    public Sprite Sprite;
}



