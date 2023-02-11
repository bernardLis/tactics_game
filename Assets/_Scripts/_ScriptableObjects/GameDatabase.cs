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


    public void Initialize() { SortItems(); }

    [Header("Cutscenes")]
    [SerializeField] Cutscene[] Cutscenes;
    public Cutscene[] GetAllCutscenes() { return Cutscenes; }

    [Header("Characters")]
    [SerializeField] Character[] StartingTroops;
    [SerializeField] Ability[] Abilities;
    [SerializeField] Item[] Items;
    [SerializeField] StatIcon[] StatIcons;
    public Character[] GetAllStarterTroops() { return StartingTroops; }
    public Ability GetAbilityById(string id) { return Abilities.FirstOrDefault(x => x.Id == id); }
    public Ability GetRandomAbility() { return Abilities[Random.Range(0, Abilities.Length)]; }
    public List<Item> GetAllItems() { return Items.ToList(); }
    public Item GetItemById(string id) { return Items.FirstOrDefault(x => x.Id == id); }
    public Item GetRandomItem() { return Items[Random.Range(0, Items.Length)]; }
    public Item GetRandomCommonItem() { return _commonItems[Random.Range(0, _commonItems.Count)]; }
    public Item GetRandomUncommonItem() { return _uncommonItems[Random.Range(0, _uncommonItems.Count)]; }
    public Item GetRandomRareItem() { return _rareItems[Random.Range(0, _rareItems.Count)]; }
    public Item GetRandomEpicItem() { return _epicItems[Random.Range(0, _epicItems.Count)]; }

    public Sprite GetStatIconByName(string name) { return StatIcons.FirstOrDefault(x => x.StatName == name).Sprite; }

    List<Item> _commonItems = new();
    List<Item> _uncommonItems = new();
    List<Item> _rareItems = new();
    List<Item> _epicItems = new();
    void SortItems()
    {
        foreach (Item i in Items)
        {
            if (i.Rarity == ItemRarity.Common)
                _commonItems.Add(i);
            if (i.Rarity == ItemRarity.Uncommon)
                _uncommonItems.Add(i);
            if (i.Rarity == ItemRarity.Rare)
                _rareItems.Add(i);
            if (i.Rarity == ItemRarity.Epic)
                _epicItems.Add(i);
        }
    }

    [Header("Dashboard")]
    [SerializeField] Sprite[] CoinSprites;
    [SerializeField] QuestRank[] QuestRanks;
    [SerializeField] Reward[] Rewards;
    [SerializeField] ReportPaper[] ReportPapers;
    [SerializeField] Element[] Elements;
    public Sprite[] LevelUpAnimationSprites;
    public Sprite[] TroopsElementAnimationSprites;
    [SerializeField] SpiceAnimations[] SpiceAnimationSprites;
    [Serializable] public class SpiceAnimations { public Sprite[] sprites; }
    [field: SerializeField] public Sprite ShopWoodSprite { get; private set; }

    public Reward GetRewardByRank(int rank) { return Rewards.FirstOrDefault(x => x.Rank == rank); }
    public Reward GetRewardByQuestRank(int rank)
    {
        List<Reward> r = Rewards.OrderBy(o => o.Rank).ToList();
        int rewardRank = Mathf.Clamp(rank + Random.Range(-1, 2), r.First().Rank, r.Last().Rank);
        return GetRewardByRank(rewardRank);
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

    public Element GetRandomElement() { return Elements[Random.Range(0, Elements.Length)]; }
    public Element GetElementByName(ElementName name) { return Elements.FirstOrDefault(x => x.ElementName == name); }

    public QuestRank GetQuestRankById(string id) { return QuestRanks.FirstOrDefault(x => x.Id == id); }
    public QuestRank GetRandomQuestRankWithMaxRank(int maxRank)
    {
        List<QuestRank> availableRanks = new();
        foreach (QuestRank r in QuestRanks)
            if (r.Rank <= maxRank)
                availableRanks.Add(r);
        return availableRanks[Random.Range(0, availableRanks.Count)];
    }

    public QuestRank GetRandomQuestRank() { return QuestRanks[Random.Range(0, QuestRanks.Length)]; }

    public ReportPaper GetRandomReportPaper() { return ReportPapers[Random.Range(0, ReportPapers.Length)]; }
    public ReportPaper GetReportPaperById(string id) { return ReportPapers.FirstOrDefault(x => x.Id == id); }
}

public enum AbilityType { Attack, Heal, Push, Buff, Create, AttackCreate }
public enum WeaponType { Any, Melee, Ranged }
public enum ItemRarity { Common, Uncommon, Rare, Epic }
public enum StatType { Power, Health, Mana, Armor, Speed }

public enum ReportType
{
    Quest, Recruit, Text, CampBuilding, Shop,
    Pawnshop, Ability, Item, SpiceRecycle, Wages, RaiseRequest
}

public enum QuestState { Pending, Delegated, Finished, Expired, RewardCollected }
public enum CampBuildingState { NotBuilt, Building, Built, Upgrading }
public enum RecruitState { Pending, Resolved, Expired }
public enum DashboardBuildingType { Desk, Camp, Abilities }
public enum ElementName { Fire, Water, Wind, Earth }

[System.Serializable]
public struct StatIcon
{
    public string StatName;
    public Sprite Sprite;
}



