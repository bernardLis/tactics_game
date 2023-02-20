using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shop : BaseScriptableObject
{
    public int RerollCost;

    CampBuildingShop _building;
    public List<Item> Items = new();

    GameManager _gameManager;


    public void CreateShop(CampBuildingShop building)
    {
        _building = building;
        _gameManager = GameManager.Instance;

        ChooseItems();

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

        RerollCost = data.RerollCost;

        Items = new();
        foreach (var item in data.ItemIds)
            Items.Add(_gameManager.GameDatabase.GetItemById(item));
    }

    public ShopData SerializeSelf()
    {
        ShopData data = new();

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
    public int RerollCost;
    public List<string> ItemIds;
}
