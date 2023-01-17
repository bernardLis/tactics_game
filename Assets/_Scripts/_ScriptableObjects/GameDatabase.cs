using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameDatabase : BaseScriptableObject
{
    public CharacterDatabase CharacterDatabase;
    public QuestDatabase QuestDatabase;

    [Header("Events")]
    [SerializeField] JourneyEvent[] AllEvents;
    public JourneyEvent[] GetAllEvents() { return AllEvents; }
    public JourneyEvent GetEventById(string id) { return AllEvents.FirstOrDefault(x => x.Id == id); }

    [Header("Cutscenes")]
    [SerializeField] Cutscene[] Cutscenes;
    public Cutscene[] GetAllCutscenes() { return Cutscenes; }

    [Header("Board")]
    [SerializeField] TilemapBiome[] Biomes;
    [SerializeField] MapVariant[] MapVariants;
    [SerializeField] Brain[] EnemyBrains;
    public TilemapBiome GetTilemapBiomeById(string id) { return Biomes.FirstOrDefault(x => x.Id == id); }
    public TilemapBiome GetRandomBiome() { return Biomes[Random.Range(0, Biomes.Length)]; }
    public MapVariant GetMapVariantById(string id) { return MapVariants.FirstOrDefault(x => x.Id == id); }
    public MapVariant GetRandomMapVariant() { return MapVariants[Random.Range(0, MapVariants.Length)]; }
    public Brain GetEnemyBrainById(string id) { return EnemyBrains.FirstOrDefault(x => x.Id == id); }
    public Brain GetRandomEnemyBrain() { return EnemyBrains[Random.Range(0, EnemyBrains.Length)]; }

    [Header("Characters")]
    [SerializeField] Character[] StarterTroops;
    [SerializeField] Equipment[] Bodies;
    [SerializeField] Weapon[] Weapons;
    [SerializeField] Ability[] Abilities;
    [SerializeField] Item[] Items;
    [SerializeField] StatIcon[] StatIcons;
    public Character[] GetAllStarterTroops() { return StarterTroops; }
    public Equipment GetBodyByName(string name) { return Bodies.FirstOrDefault(x => x.name == name); }
    public Weapon GetRandomWeapon() { return Weapons[Random.Range(0, Weapons.Length)]; }
    public Weapon GetWeaponByName(string name) { return Weapons.FirstOrDefault(x => x.name == name); }
    public Ability GetAbilityById(string id) { return Abilities.FirstOrDefault(x => x.Id == id); }
    public Ability GetRandomAbility() { return Abilities[Random.Range(0, Abilities.Length)]; }
    public Item GetItemById(string id) { return Items.FirstOrDefault(x => x.Id == id); }
    public Item GetRandomItem() { return Items[Random.Range(0, Items.Length)]; }
    public Sprite GetStatIconByName(string name) { return StatIcons.FirstOrDefault(x => x.StatName == name).Sprite; }


    [Header("Battle")]
    [SerializeField] BattleLogLineIcon[] BattleLogLineIcons;
    public Sprite GetBattleLogLineIconByType(BattleLogLineType type) { return BattleLogLineIcons.FirstOrDefault(x => x.BattleLogLineType == type).Sprite; }

    [Header("Dashboard")]
    [SerializeField] Sprite[] CoinSprites;
    [SerializeField] QuestRank[] QuestRanks;
    [SerializeField] RewardChest[] RewardChests;
    [SerializeField] ReportPaper[] ReportPapers;
    [SerializeField] Element[] Elements;
    public Sprite[] LevelUpAnimationSprites;
    public Sprite[] TroopsElementAnimationSprites;
    [SerializeField] SpiceAnimations[] SpiceAnimationSprites;
    [Serializable] public class SpiceAnimations { public Sprite[] sprites; }
    [field: SerializeField] public Sprite ShopWoodSprite { get; private set; }


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

    public Element GetRandomElement() { return Elements[Random.Range(0, Elements.Length)]; }
    public Element GetElementByName(ElementName name) { return Elements.FirstOrDefault(x => x.ElementName == name); }

    public QuestRank GetQuestRankById(string id) { return QuestRanks.FirstOrDefault(x => x.Id == id); }
    public QuestRank GetRandomQuestRank() { return QuestRanks[Random.Range(0, QuestRanks.Length)]; }

    public RewardChest GetRandomRewardChest() { return RewardChests[Random.Range(0, RewardChests.Length)]; }

    public ReportPaper GetRandomReportPaper() { return ReportPapers[Random.Range(0, ReportPapers.Length)]; }
    public ReportPaper GetReportPaperById(string id) { return ReportPapers.FirstOrDefault(x => x.Id == id); }
}

public enum AbilityType { Attack, Heal, Push, Buff, Create, AttackCreate }
public enum WeaponType { Any, Melee, Ranged }
public enum ItemRarity { Common, Uncommon, Rare, Epic }
public enum BattleGoal { DefeatAllEnemies } // TODO: implement other battle goals (defeat the leader, hold position, ...)
public enum JourneyNodeType { Start, End, Battle, Knowledge, Chest, Shop, Fire, Boss, Event }
public enum MapType { None, Circle, River, Lake, Hourglass }
public enum TileMapObjectType { Outer, Obstacle, PushableObstacle }
public enum CharacterState { None, Selected, Moved, SelectingInteractionTarget }
public enum EnemySpawnDirection { Left, Right, Top, Bottom }
public enum BattleState { MapBuilding, Deployment, PlayerTurn, EnemyTurn, Won, Lost }
public enum StatType { Power, MaxHealth, MaxMana, Armor, MovementRange }
public enum BattleLogLineType { Ability, Damage, Death, Info, Status }
public enum ReportType
{
    Quest, Recruit, Text, CampBuilding, Shop,
    Pawnshop, Ability, SpiceRecycle, Wages, RaiseRequest
}
public enum QuestState { Pending, Delegated, Finished, Expired, RewardCollected }
public enum CampBuildingState { Pending, Started, Finished }
public enum RecruitState { Pending, Resolved, Expired }
public enum DashboardBuildingType { Desk, Camp, Abilities }
public enum ElementName { Fire, Water, Wind, Earth }

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




