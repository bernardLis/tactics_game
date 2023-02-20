using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shop : BaseScriptableObject
{
    public DateTime DateTimeAdded;
    public DateTime DateTimeExpired;

    public int RerollCost;

    CampBuildingShop _building;
    public List<Item> Items = new();

    GameManager _gameManager;


    public void CreateShop(CampBuildingShop building)
    {
        _building = building;
        _gameManager = GameManager.Instance;

        ChooseItems();
        DateTimeAdded = ScriptableObject.CreateInstance<DateTime>();
        DateTimeAdded = _gameManager.GetCurrentDateTime();

        DateTimeExpired = ScriptableObject.CreateInstance<DateTime>();
        DateTimeExpired.Day = _gameManager.Day + Random.Range(2, 5);

        RerollCost = 200;
    }

    public void ItemBought(Item item)
    {
        Items.Remove(item);
        _gameManager.SaveJsonData();
    }

    void ChooseItems()
    {
        Items.Clear();
        for (int i = 0; i < 2; i++)
        {
            Item item = _building.GetRandomItem();
            Items.Add(item);
        }
    }

    public void RerollItems() { ChooseItems(); }

    public void LoadFromData(ShopData data)
    {
        _gameManager = GameManager.Instance;
        _building = _gameManager.GetComponent<BuildingManager>().ShopBuilding;

        DateTimeAdded = ScriptableObject.CreateInstance<DateTime>();
        DateTimeAdded.LoadFromData(data.DateTimeAdded);

        DateTimeExpired = ScriptableObject.CreateInstance<DateTime>();
        DateTimeExpired.LoadFromData(data.DateTimeExpired);

        RerollCost = data.RerollCost;

        Items = new();
        foreach (var item in data.ItemIds)
            Items.Add(_gameManager.GameDatabase.GetItemById(item));
    }

    public ShopData SerializeSelf()
    {
        ShopData data = new();
        data.DateTimeAdded = DateTimeAdded.SerializeSelf();
        data.DateTimeExpired = DateTimeExpired.SerializeSelf();

        data.RerollCost = RerollCost;
        data.ItemIds = new();
        foreach (var item in Items)
            data.ItemIds.Add(item.Id);
        return data;
    }
}

[Serializable]
public struct ShopData
{
    public DateTimeData DateTimeAdded;
    public DateTimeData DateTimeExpired;
    public int RerollCost;
    public List<string> ItemIds;
}
