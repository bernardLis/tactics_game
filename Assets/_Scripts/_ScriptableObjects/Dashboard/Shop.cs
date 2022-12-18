using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shop : BaseScriptableObject
{
    public int DayAdded;
    public int Duration;
    public int RerollCost;
    public int AddDayCost;
    public List<Item> Items = new();

    GameManager _gameManager;


    public event Action OnDurationChanged;

    public void CreateShop()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
        ChooseItems();

        DayAdded = _gameManager.Day;
        Duration = Random.Range(1, 4);
        RerollCost = 200;
        AddDayCost = 200;
    }

    void OnDayPassed(int day)
    {
        Duration--;
        OnDurationChanged?.Invoke();
        if (Duration <= 0)
            CloseShop();
    }

    public void ItemBought(Item item)
    {
        Items.Remove(item);
        _gameManager.SaveJsonData();
    }

    void CloseShop()
    {
        Debug.Log($"closing shop");
    }

    void ChooseItems()
    {
        Items.Clear();
        for (int i = 0; i < 2; i++)
        {
            Item item = _gameManager.GameDatabase.GetRandomItem();
            Items.Add(item);
        }
    }

    public void RerollItems() { ChooseItems(); }

    public void LoadFromData(ShopData data)
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;

        DayAdded = data.DayAdded;
        Duration = data.Duration;
        RerollCost = data.RerollCost;
        AddDayCost = data.AddDayCost;
        Items = new();
        foreach (var item in data.ItemIds)
            Items.Add(_gameManager.GameDatabase.GetItemById(item));
    }

    public ShopData SerializeSelf()
    {
        ShopData sd = new();
        sd.DayAdded = DayAdded;
        sd.Duration = Duration;
        sd.RerollCost = RerollCost;
        sd.AddDayCost = AddDayCost;
        sd.ItemIds = new();
        foreach (var item in Items)
            sd.ItemIds.Add(item.Id);
        return sd;
    }
}

[Serializable]
public struct ShopData
{
    public int DayAdded;
    public int Duration;
    public int RerollCost;
    public int AddDayCost;
    public List<string> ItemIds;
}
