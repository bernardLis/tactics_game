using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Hospital")]
public class CampBuildingHospital : CampBuilding
{
    public List<CampUpgradeHospital> Upgrades = new();

    public CampUpgradeHospital GetUpgradeByRank(int rank)
    {
        return Upgrades.FirstOrDefault(x => x.UpgradeRank == rank);
    }
}

[System.Serializable]
public struct CampUpgradeHospital
{
    public int UpgradeRank;
    public int MaxDaysDisabled;
}
