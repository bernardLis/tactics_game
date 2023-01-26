using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Shop")]
public class CampBuildingShop : CampBuilding
{
    public List<CampShopUpgrade> CampShopUpgrades = new();

    List<Item> commonItems = new();
    List<Item> uncommonItems = new();
    List<Item> rareItems = new();
    List<Item> epicItems = new();

    public override void Initialize()
    {
        base.Initialize();
        foreach (Item i in _gameManager.GameDatabase.GetAllItems())
        {
            if (i.Rarity == ItemRarity.Common)
                commonItems.Add(i);
            if (i.Rarity == ItemRarity.Uncommon)
                uncommonItems.Add(i);
            if (i.Rarity == ItemRarity.Rare)
                rareItems.Add(i);
            if (i.Rarity == ItemRarity.Epic)
                epicItems.Add(i);
        }
    }
    
    public CampShopUpgrade GetUpgradeByRank(int rank)
    {
        return CampShopUpgrades.FirstOrDefault(x => x.UpgradeRank == rank);
    }

    public Item GetRandomItem()
    {
        float v = Random.value;
        CampShopUpgrade upgrade = GetUpgradeByRank(UpgradeRank);
        if (v < upgrade.EpicItemChance)
            return epicItems[Random.Range(0, epicItems.Count)];
        if (v < upgrade.RareItemChance)
            return rareItems[Random.Range(0, rareItems.Count)];
        if (v < upgrade.UncommonItemChance)
            return uncommonItems[Random.Range(0, uncommonItems.Count)];

        return commonItems[Random.Range(0, commonItems.Count)];
    }
}

[System.Serializable]
public struct CampShopUpgrade
{
    public int UpgradeRank;
    public float UncommonItemChance;
    public float RareItemChance;
    public float EpicItemChance;
}
