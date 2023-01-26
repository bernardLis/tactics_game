using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Spice Recycler")]
public class CampBuildingSpiceRecycler : CampBuilding
{

    public List<CampSpiceRecyclerUpgrade> CampSpiceRecyclerUpgrades = new();

    public CampSpiceRecyclerUpgrade GetUpgradeByRank(int rank)
    {
        return CampSpiceRecyclerUpgrades.FirstOrDefault(x => x.UpgradeRank == rank);
    }
}

[System.Serializable]
public struct CampSpiceRecyclerUpgrade
{
    public int UpgradeRank;
    public float ChanceToVisit;
}
