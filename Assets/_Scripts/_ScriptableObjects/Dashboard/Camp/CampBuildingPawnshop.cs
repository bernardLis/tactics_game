using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Pawnshop")]
public class CampBuildingPawnshop : CampBuilding
{

    public List<CampPawnshopUpgrade> CampPawnshopUpgrades = new();

    public float GetVisitChance()
    {
        return CampPawnshopUpgrades.FirstOrDefault(x => x.UpgradeRank == UpgradeRank).ChanceToVisit;
    }

    public float GetNextUpgradeVisitChance()
    {
        if (UpgradeRank + 1 > UpgradeRange.y)
            return GetVisitChance();
        return CampPawnshopUpgrades.FirstOrDefault(x => x.UpgradeRank == UpgradeRank + 1).ChanceToVisit;
    }
}

[System.Serializable]
public struct CampPawnshopUpgrade
{
    public int UpgradeRank;
    public float ChanceToVisit;
}
