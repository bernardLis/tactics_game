using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : PersistentSingleton<GameManager>, ISavable
{
    SaveData _originalSaveData;

    LevelLoader _levelLoader;

    public GameDatabase GameDatabase;

    // global data
    public bool WasTutorialPlayed { get; private set; }
    public int Seed { get; private set; }

    public int Day { get; private set; }
    public int Gold { get; private set; }

    public List<Quest> NewQuests = new();
    public List<Quest> AvailableQuests = new();

    public List<Item> ShopItems = new();
    public int ShopRerollPrice { get; private set; }

    public List<Character> PlayerTroops = new();
    [HideInInspector] public List<Item> PlayerItemPouch = new();
    [HideInInspector] public List<Ability> PlayerAbilityPouch = new();

    public List<Report> Reports = new();

    public int CutsceneIndexToPlay = 0; // TODO: this is wrong, but for now it is ok

    public Quest ActiveQuest;

    public event Action<int> OnDayPassed;
    public event Action<int> OnGoldChanged;
    public event Action<int> OnShopRerollPriceChanged;
    public event Action<string> OnLevelLoaded;

    protected override void Awake()
    {
        base.Awake();
        _levelLoader = GetComponent<LevelLoader>();

        // global save per 'game'
        if (PlayerPrefs.GetString("saveName").Length == 0)
            CreateNewSaveFile();
        else
            LoadFromSaveFile();
    }

    public void Play()
    {
        if (PlayerPrefs.GetString("saveName").Length == 0) // TODO: this does not work...
            LoadLevel(Scenes.Cutscene);
        else
            LoadLevel(Scenes.Dashboard);
    }


    /* RESOURCES */
    public void PassDay()
    {
        Day += 1;

        if (Day % 7 == 0) // shop resets every 7th day
        {
            ChooseShopItems();
            ChangeShopRerollPrice(2);
        }

        PayMaintenance();
        AddRandomQuest();
        ResolveDelegatedQuests();

        OnDayPassed?.Invoke(Day);
        SaveJsonData();
    }

    void PayMaintenance() { ChangeGoldValue(-GetCurrentMaintenanceCost()); }

    public int GetCurrentMaintenanceCost() { return PlayerTroops.Count * 2; }

    void AddRandomQuest()
    {
        Quest q = ScriptableObject.CreateInstance<Quest>();
        q.CreateRandom();
        NewQuests.Add(q);
    }

    void ResolveDelegatedQuests()
    {
        foreach (Quest q in AvailableQuests)
        {
            if (!q.IsDelegated)
                continue;

            if (q.CountDaysLeft() > 0)
                continue;

            // roll for success // HERE: 
            // if success get rewards, unlock characters
            // else no rewards, characters are locked for x days
        }

    }

    public void ChangeGoldValue(int o)
    {
        if (o == 0)
            return;

        Gold += o;
        OnGoldChanged?.Invoke(Gold);
        SaveJsonData();
    }

    /* Shop */
    public void RemoveItemFromShop(Item item)
    {
        ShopItems.Remove(item);
        SaveJsonData();
    }

    public void ChooseShopItems()
    {
        ShopItems = new();
        for (int i = 0; i < 6; i++)
        {
            Item item = GameDatabase.GetRandomItem();
            ShopItems.Add(item);
        }
    }

    public void ChangeShopRerollPrice(int newValue)
    {
        ShopRerollPrice = newValue;
        OnShopRerollPriceChanged?.Invoke(newValue);
        SaveJsonData();
    }

    /* Troops & pouches */
    public void AddCharacterToTroops(Character character)
    {
        PlayerTroops.Add(character);
        SaveJsonData();
    }

    public void AddItemToPouch(Item item)
    {
        PlayerItemPouch.Add(item);
        SaveJsonData();
    }

    public void RemoveItemFromPouch(Item item)
    {
        PlayerItemPouch.Remove(item);
        SaveJsonData();
    }

    public void AddAbilityToPouch(Ability ability)
    {
        PlayerAbilityPouch.Add(ability);
        SaveJsonData();
    }

    public void RemoveAbilityFromPouch(Ability ability)
    {
        PlayerAbilityPouch.Remove(ability);
        SaveJsonData();
    }

    /* LEVELS */
    public void SetWasTutorialPlayed(bool was)
    {
        WasTutorialPlayed = was;
        SaveJsonData();
    }

    public void StartBattle(Quest quest)
    {
        ActiveQuest = quest;
        LoadLevel(ActiveQuest.SceneToLoad);
    }

    public void LoadLevel(string level)
    {
        _levelLoader.LoadLevel(level);
        OnLevelLoaded?.Invoke(level);
    }

    /*************
    * Saving and Loading
    * https://www.youtube.com/watch?v=uD7y4T4PVk0
    */

    List<Character> CreatePlayerTroops() // for the new save
    {
        List<Character> instantiatedTroops = new();

        List<Character> playerCharacters = new(GameDatabase.GetAllStarterTroops());
        PlayerTroops = new();
        foreach (Character character in playerCharacters)
        {
            Character instance = Instantiate(character);
            OnDayPassed += instance.OnDayPassed;
            instantiatedTroops.Add(instance);
        }

        return instantiatedTroops;
    }

    void CreateNewSaveFile()
    {
        Seed = System.Environment.TickCount;

        Day = 0;
        Gold = 0;

        ChooseShopItems();
        ShopRerollPrice = 2;

        PlayerTroops = CreatePlayerTroops();

        // TODO: HERE: for now, I could hand craft 3 first quests or somethinmg...
        for (int i = 0; i < 3; i++)
        {
            Quest q = ScriptableObject.CreateInstance<Quest>();
            q.CreateRandom();
            AvailableQuests.Add(q);
        }

        // new save
        string guid = System.Guid.NewGuid().ToString();
        string fileName = guid + ".dat";
        FileManager.CreateFile(fileName);
        PlayerPrefs.SetString("saveName", fileName);
        PlayerPrefs.Save();

        SaveJsonData();
    }

    public void LoadFromSaveFile() { LoadJsonData(PlayerPrefs.GetString("saveName")); }

    public void SaveJsonData()
    {
        SaveData sd = new SaveData();
        PopulateSaveData(sd);
        if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), sd.ToJson()))
            Debug.Log("Save successful");
    }

    public void PopulateSaveData(SaveData saveData)
    {
        // global data
        saveData.WasTutorialPlayed = WasTutorialPlayed;
        saveData.Seed = Seed;

        saveData.Day = Day;
        saveData.Gold = Gold;

        saveData.ShopItems = PopulateShopItems();
        saveData.ShopRerollPrice = ShopRerollPrice;

        saveData.PlayerTroops = PopulateCharacters();
        saveData.ItemPouch = PopulateItemPouch();
        saveData.AbilityPouch = PopulateAbilityPouch();

        saveData.NewQuests = PopulateNewQuests();
        saveData.AvailableQuests = PopulateAvailableQuests();
    }

    List<string> PopulateShopItems()
    {
        List<string> itemReferenceIds = new();
        foreach (Item i in ShopItems)
            itemReferenceIds.Add(i.ReferenceID);

        return itemReferenceIds;
    }

    List<CharacterData> PopulateCharacters()
    {
        List<CharacterData> charDatas = new();
        foreach (Character c in PlayerTroops)
            charDatas.Add(c.SerializeSelf());

        return charDatas;
    }

    List<string> PopulateItemPouch()
    {
        List<string> itemReferenceIds = new();
        foreach (Item i in PlayerItemPouch)
            itemReferenceIds.Add(i.ReferenceID);

        return itemReferenceIds;
    }

    List<string> PopulateAbilityPouch()
    {
        List<string> abilityReferenceIds = new();
        foreach (Ability a in PlayerAbilityPouch)
            abilityReferenceIds.Add(a.ReferenceID);

        return abilityReferenceIds;
    }

    List<QuestData> PopulateNewQuests()
    {
        List<QuestData> quests = new();
        foreach (Quest q in NewQuests)
            quests.Add(q.SerializeSelf());
        return quests;
    }

    List<QuestData> PopulateAvailableQuests()
    {
        List<QuestData> quests = new();
        foreach (Quest q in AvailableQuests)
            quests.Add(q.SerializeSelf());
        return quests;
    }

    void LoadJsonData(string fileName)
    {
        if (FileManager.LoadFromFile(fileName, out var json))
        {
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);
            LoadFromSaveData(sd);
        }
    }

    public void LoadFromSaveData(SaveData saveData)
    {
        _originalSaveData = saveData; // stored for later

        // global data
        WasTutorialPlayed = saveData.WasTutorialPlayed;
        Seed = saveData.Seed;

        Day = saveData.Day;
        Gold = saveData.Gold;

        ShopRerollPrice = saveData.ShopRerollPrice;
        ShopItems = new();
        foreach (string itemReferenceId in saveData.ShopItems)
            ShopItems.Add(GameDatabase.GetItemByReferenceId(itemReferenceId));

        PlayerTroops = new();
        foreach (CharacterData data in saveData.PlayerTroops)
        {
            Character playerCharacter = (Character)ScriptableObject.CreateInstance<Character>();
            playerCharacter.CreateFromData(data);
            PlayerTroops.Add(playerCharacter);
        }

        PlayerItemPouch = new();
        foreach (string itemReferenceId in saveData.ItemPouch)
            PlayerItemPouch.Add(GameDatabase.GetItemByReferenceId(itemReferenceId));

        PlayerAbilityPouch = new();
        foreach (string abilityReferenceId in saveData.AbilityPouch)
            PlayerAbilityPouch.Add(GameDatabase.GetAbilityByReferenceId(abilityReferenceId));

        LoadQuests(saveData);
    }

    void LoadQuests(SaveData saveData)
    {
        NewQuests = new();
        foreach (QuestData qd in saveData.NewQuests)
        {
            Quest quest = ScriptableObject.CreateInstance<Quest>();
            quest.CreateFromData(qd);
            NewQuests.Add(quest);
        }

        AvailableQuests = new();
        foreach (QuestData qd in saveData.AvailableQuests)
        {
            Quest quest = ScriptableObject.CreateInstance<Quest>();
            quest.CreateFromData(qd);
            AvailableQuests.Add(quest);
        }
    }


    public void ClearSaveData()
    {
        PlayerPrefs.DeleteAll();

        WasTutorialPlayed = false;
        Seed = System.Environment.TickCount;

        Day = 0;
        Gold = 0;

        ChooseShopItems();
        ShopRerollPrice = 2;

        PlayerTroops = CreatePlayerTroops();
        PlayerItemPouch = new();
        PlayerAbilityPouch = new();

        CutsceneIndexToPlay = 0; // TODO: wrong but it's ok for now.

        NewQuests = new();
        AvailableQuests = new();

        if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), ""))
            Debug.Log("Cleared active save");

        LoadLevel(Scenes.MainMenu);
    }

}
