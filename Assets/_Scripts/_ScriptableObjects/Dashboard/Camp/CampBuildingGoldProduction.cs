using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Gold Production")]
public class CampBuildingGoldProduction : CampBuilding
{
    public List<CampUpgradeGoldProduction> Upgrades = new();

    public override int GetMaxUpgradeRank()
    {
        return Upgrades.OrderByDescending(x => x.UpgradeRank).First().UpgradeRank;
    }

    public CampUpgradeGoldProduction GetUpgradeByRank(int rank)
    {
        return Upgrades.FirstOrDefault(x => x.UpgradeRank == rank);
    }

    public void Produce()
    {
        if (_gameManager.Day % 7 == 0)
            _gameManager.ChangeGoldValue(GetUpgradeByRank(UpgradeRank).GoldPerWeek);
    }
}

[System.Serializable]
public struct CampUpgradeGoldProduction
{
    public int UpgradeRank;
    public int GoldPerWeek;
}
