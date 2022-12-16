using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : BaseScriptableObject
{
    public int DayAdded;
    public int Duration;
    public int RerollCost;
    public int AddDayCost;
    public List<Item> Items = new();

    GameManager _gameManager;
    public void CreateShop()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
        ChooseItems();

    }

    void OnDayPassed(int day)
    {
        Duration--;
        if (Duration <= 0)
            CloseShop();
    }

    void CloseShop()
    {
        Debug.Log($"closing shop");
    }

    void ChooseItems()
    {
        Debug.Log($"choosing items");
    }

    public void AddDay()
    {
        Duration++;
        _gameManager.SaveJsonData();
    }

    public void RerollItems()
    {
        ChooseItems();

    }

    public void CreateFromData(ShopData data)
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
