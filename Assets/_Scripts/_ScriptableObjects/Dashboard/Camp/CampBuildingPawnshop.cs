using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Pawnshop")]
public class CampBuildingPawnshop : CampBuilding
{
    public List<CampUpgradePawnshop> Upgrades = new();

    public override int GetMaxUpgradeRank()
    {
        return Upgrades.OrderByDescending(x => x.UpgradeRank).First().UpgradeRank;
    }

    public CampUpgradePawnshop GetUpgradeByRank(int rank)
    {
        return Upgrades.FirstOrDefault(x => x.UpgradeRank == rank);
    }
}

[System.Serializable]
public struct CampUpgradePawnshop
{
    public int UpgradeRank;
    public float ChanceToVisit;
}
