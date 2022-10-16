using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameDatabase : BaseScriptableObject
{
    [Header("Events")]
    [SerializeField] JourneyEvent[] AllEvents;
    public JourneyEvent[] GetAllEvents() { return AllEvents; }

    [Header("Cutscenes")]
    [SerializeField] Cutscene[] Cutscenes;
    public Cutscene[] GetAllCutscenes() { return Cutscenes; }


    [Header("Board")]
    [SerializeField] Sprite[] BattleNodeIcons;
    [SerializeField] TilemapBiome[] Biomes;
    [SerializeField] MapVariant[] MapVariants;
    [SerializeField] Brain[] EnemyBrains;
    public Sprite GetRandomBattleNodeIcon() { return BattleNodeIcons[Random.Range(0, BattleNodeIcons.Length)]; }
    public TilemapBiome GetTilemapBiomeById(string id) { return Biomes.FirstOrDefault(x => x.Id == id); }
    public TilemapBiome GetRandomBiome() { return Biomes[Random.Range(0, Biomes.Length)]; }
    public MapVariant GetMapVariantById(string id) { return MapVariants.FirstOrDefault(x => x.Id == id); }
    public MapVariant GetRandomMapVariant() { return MapVariants[Random.Range(0, MapVariants.Length)]; }
    public Brain GetEnemyBrainById(string id) { return EnemyBrains.FirstOrDefault(x => x.Id == id); }
    public Brain GetRandomEnemyBrain() { return EnemyBrains[Random.Range(0, EnemyBrains.Length)]; }

    [Header("Characters")]
    [SerializeField] Character[] StarterTroops;
    [SerializeField] PortraitEntry[] Portraits;
    [SerializeField] Equipment[] Bodies;
    [SerializeField] Weapon[] Weapons;
    [SerializeField] Ability[] Abilities;
    [SerializeField] Item[] Items;
    [SerializeField] StatIcon[] StatIcons;
    [Header("Battle")]
    [SerializeField] BattleLogLineIcon[] BattleLogLineIcons;

    [SerializeField] public RewardChest[] RewardChests;
    [SerializeField] Sprite[] CoinSprites;

    public Character[] GetAllStarterTroops() { return StarterTroops; }

    public Sprite GetPortraitById(string id) { return Portraits.FirstOrDefault(x => x.ReferenceID == id).Sprite; }

    public Equipment GetBodyByName(string name) { return Bodies.FirstOrDefault(x => x.name == name); }

    public Weapon GetWeaponByName(string name) { return Weapons.FirstOrDefault(x => x.name == name); }

    public JourneyEvent GetEventById(string id) { return AllEvents.FirstOrDefault(x => x.Id == id); }

    // TODO: I am not certain if this reference ID and normal ID is a smart move.
    public Ability GetAbilityById(string id) { return Abilities.FirstOrDefault(x => x.Id == id); }
    public Ability GetAbilityByReferenceId(string id) { return Abilities.FirstOrDefault(x => x.ReferenceID == id); }
    public Ability GetRandomAbility() { return Abilities[Random.Range(0, Abilities.Length)]; }

    public Item GetItemByReference(string id) { return Items.FirstOrDefault(x => x.ReferenceID == id); }
    public Item GetRandomItem() { return Items[Random.Range(0, Items.Length)]; }

    public Sprite GetStatIconByName(string name) { return StatIcons.FirstOrDefault(x => x.StatName == name).Sprite; }

    public Sprite GetBattleLogLineIconByType(BattleLogLineType type) { return BattleLogLineIcons.FirstOrDefault(x => x.BattleLogLineType == type).Sprite; }

    public RewardChest GetRandomRewardChest() { return RewardChests[Random.Range(0, RewardChests.Length)]; }

    public Sprite GetCoinSprite(int amount)
    {
        int index = 0;
        // TODO: something smarter
        if (amount >= 0 && amount <= 3)
            index = 0;
        if (amount >= 4 && amount <= 6)
            index = 1;
        if (amount >= 7 && amount <= 9)
            index = 2;
        if (amount >= 10 && amount <= 12)
            index = 3;
        if (amount >= 13)
            index = 4;

        return CoinSprites[index];
    }
}

public enum AbilityType { Attack, Heal, Push, Buff, Create, AttackCreate }
public enum WeaponType { Any, Melee, Ranged }
public enum UpgradeType { Character, Run }
public enum ItemRaririty { Common, Magic, Rare, Epic }
public enum BattleGoal { DefeatAllEnemies } // TODO: implement other battle goals (defeat the leader, hold position, ...)
public enum JourneyNodeType { Start, End, Battle, Knowledge, Chest, Shop, Fire, Boss, Event }
public enum MapType { None, Circle, River, Lake, Hourglass }
public enum TileMapObjectType { Outer, Obstacle, PushableObstacle }
public enum CharacterState { None, Selected, Moved, SelectingInteractionTarget }
public enum EnemySpawnDirection { Left, Right, Top, Bottom }
public enum BattleState { MapBuilding, Deployment, PlayerTurn, EnemyTurn, Won, Lost }
public enum StatType { Power, MaxHealth, MaxMana, Armor, MovementRange }
public enum BattleLogLineType { Ability, Damage, Death, Info, Status }


[System.Serializable]
public struct PortraitEntry
{
    public string ReferenceID;
    public Sprite Sprite;
}

[System.Serializable]
public struct StatIcon
{
    public string StatName;
    public Sprite Sprite;
}

[System.Serializable]
public struct BattleLogLineIcon
{
    public BattleLogLineType BattleLogLineType;
    public Sprite Sprite;
}

[System.Serializable]
public struct RewardChest
{
    public Sprite[] Idle;
    public Sprite[] Open;
}



