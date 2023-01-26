using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Pawnshop")]
public class CampBuildingPawnshop : CampBuilding
{
    public List<CampPawnshopUpgrade> CampPawnshopUpgrades = new();

    public CampPawnshopUpgrade GetUpgradeByRank(int rank)
    {
        return CampPawnshopUpgrades.FirstOrDefault(x => x.UpgradeRank == rank);
    }
}

[System.Serializable]
public struct CampPawnshopUpgrade
{
    public int UpgradeRank;
    public float ChanceToVisit;
}
