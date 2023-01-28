using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Spice Production")]
public class CampBuildingSpiceProduction : CampBuilding
{
    public List<CampUpgradeSpiceProduction> Upgrades = new();

    public override int GetMaxUpgradeRank()
    {
        return Upgrades.OrderByDescending(x => x.UpgradeRank).First().UpgradeRank;
    }

    public CampUpgradeSpiceProduction GetUpgradeByRank(int rank)
    {
        return Upgrades.FirstOrDefault(x => x.UpgradeRank == rank);
    }

    public void Produce()
    {
        if (_gameManager.Day % 7 == 0)
            _gameManager.ChangeSpiceValue(GetUpgradeByRank(UpgradeRank).SpicePerWeek);
    }
}

[System.Serializable]
public struct CampUpgradeSpiceProduction
{
    public int UpgradeRank;
    public int SpicePerWeek;
}
