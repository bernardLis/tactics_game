using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Shop")]
public class CampBuildingShop : CampBuilding
{
    public List<CampUpgradeShop> Upgrades = new();

    public override void Initialize()
    {
        base.Initialize();
    }

    public override int GetMaxUpgradeRank()
    {
        return Upgrades.OrderByDescending(x => x.UpgradeRank).First().UpgradeRank;
    }

    public CampUpgradeShop GetUpgradeByRank(int rank)
    {
        return Upgrades.FirstOrDefault(x => x.UpgradeRank == rank);
    }

    public Item GetRandomItem()
    {
        float v = Random.value;
        CampUpgradeShop upgrade = GetUpgradeByRank(UpgradeRank);
        if (v < upgrade.EpicItemChance)
            return _gameManager.GameDatabase.GetRandomEpicItem();
        if (v < upgrade.RareItemChance)
            return _gameManager.GameDatabase.GetRandomRareItem();
        if (v < upgrade.UncommonItemChance)
            return _gameManager.GameDatabase.GetRandomUncommonItem();

        return _gameManager.GameDatabase.GetRandomCommonItem();
    }
}

[System.Serializable]
public struct CampUpgradeShop
{
    public int UpgradeRank;
    public float UncommonItemChance;
    public float RareItemChance;
    public float EpicItemChance;
}
