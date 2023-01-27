using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Spice Recycler")]
public class CampBuildingSpiceRecycler : CampBuilding
{
    public List<CampUpgradeSpiceRecycler> Upgrades = new();

    public override int GetMaxUpgradeRank()
    {
        return Upgrades.OrderByDescending(x => x.UpgradeRank).First().UpgradeRank;
    }

    public CampUpgradeSpiceRecycler GetUpgradeByRank(int rank)
    {
        return Upgrades.FirstOrDefault(x => x.UpgradeRank == rank);
    }
}

[System.Serializable]
public struct CampUpgradeSpiceRecycler
{
    public int UpgradeRank;
    public float ChanceToVisit;
}
