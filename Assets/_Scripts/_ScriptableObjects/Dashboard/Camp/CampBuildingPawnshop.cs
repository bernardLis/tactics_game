using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Pawnshop")]
public class CampBuildingPawnshop : CampBuilding
{

    public List<CampPawnshopUpgrade> CampPawnshopUpgrades = new();

    public float GetPawnshopVisitChance()
    {
        return CampPawnshopUpgrades.FirstOrDefault(x => x.UpgradeRank == UpgradeRank).ChanceToVisit;
    }

    public float GetNextUpgradePawnshopVisitChance()
    {
        if (UpgradeRank + 1 > UpgradeRange.y)
            return GetPawnshopVisitChance();
        return CampPawnshopUpgrades.FirstOrDefault(x => x.UpgradeRank == UpgradeRank + 1).ChanceToVisit;
    }


    public override void FinishBuilding() { base.FinishBuilding(); }

    public override void Upgrade()
    {
        base.Upgrade();
    }
}

[System.Serializable]
public struct CampPawnshopUpgrade
{
    public int UpgradeRank;
    public float ChanceToVisit;
}
