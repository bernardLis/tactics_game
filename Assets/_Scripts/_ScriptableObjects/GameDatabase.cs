using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameDatabase : BaseScriptableObject
{

    [Header("Global Upgrades")]
    [SerializeField] GlobalUpgrade[] GlobalUpgrades;
    public GlobalUpgrade[] GetAllGlobalUpgrades() { return GlobalUpgrades; }
    public GlobalUpgrade GetGlobalUpgradeById(string id)
    {
        GlobalUpgrade gu = GlobalUpgrades.First(x => x.Id == id);
        if (gu == null)
            Debug.LogError($"Global upgrade with id: {id} does not exist");
        return gu;
    }

    [Header("Events")]
    [SerializeField] JourneyEvent[] AllEvents;
    public JourneyEvent[] GetAllEvents() { return AllEvents; }

    [Header("Cutscenes")]
    [SerializeField] Cutscene[] Cutscenes;
    public Cutscene[] GetAllCutscenes() { return Cutscenes; }


    [Header("Board")]
    [SerializeField] TilemapBiome[] Biomes;
    [SerializeField] MapVariant[] MapVariants;
    [SerializeField] Brain[] EnemyBrains;

    public TilemapBiome GetRandomBiome() { return Biomes[Random.Range(0, Biomes.Length)]; }
    public MapVariant GetRandomMapVariant() { return MapVariants[Random.Range(0, MapVariants.Length)]; }
    public Brain GetRandomEnemyBrain() { return EnemyBrains[Random.Range(0, EnemyBrains.Length)]; }

    [Header("Characters")]
    [SerializeField] Character[] StarterTroops;
    [SerializeField] PortraitEntry[] Portraits;
    [SerializeField] Equipment[] Bodies;
    [SerializeField] Weapon[] Weapons;
    [SerializeField] Ability[] Abilities;
    [SerializeField] Item[] Items;
    [SerializeField] StatIcon[] StatIcons;
    [SerializeField] BattleLogLineIcon[] BattleLogLineIcons;

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
}

public enum AbilityType { Attack, Heal, Push, Buff, Create, AttackCreate }
public enum WeaponType { Any, Melee, Ranged }
public enum UpgradeType { Character, Run }
public enum ItemRaririty { Common, Magic, Rare, Epic }
public enum BattleGoal { DefeatAllEnemies } // TODO: implement other battle goals (defeat the leader, hold position, ...)
public enum JourneyNodeType { Start, End, Battle, Knowledge, Chest, Shop, Fire, Boss, Event }
public enum MapType { None, Circle, River, Lake, Hourglass }
public enum TileMapObjectType { Outer, Obstacle, PushableObstacle }
public enum CharacterState { None, Selected, Moved, SelectingInteractionTarget, SelectingFaceDir, ConfirmingInteraction }
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


