using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Spice Recycler")]
public class CampBuildingSpiceRecycler : CampBuilding
{

    public List<CampSpiceRecyclerUpgrade> CampSpiceRecyclerUpgrades = new();

    public float GetVisitChance()
    {
        return CampSpiceRecyclerUpgrades.FirstOrDefault(x => x.UpgradeRank == UpgradeRank).ChanceToVisit;
    }

    public float GetNextUpgradeVisitChance()
    {
        if (UpgradeRank + 1 > UpgradeRange.y)
            return GetVisitChance();
        return CampSpiceRecyclerUpgrades.FirstOrDefault(x => x.UpgradeRank == UpgradeRank + 1).ChanceToVisit;
    }
}

[System.Serializable]
public struct CampSpiceRecyclerUpgrade
{
    public int UpgradeRank;
    public float ChanceToVisit;
}
