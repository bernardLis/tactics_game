using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Recruiting")]
public class CampBuildingRecruiting : CampBuilding
{
    public List<CampUpgradeRecruiting> Upgrades = new();

    public CampUpgradeRecruiting GetUpgradeByRank(int rank)
    {
        return Upgrades.FirstOrDefault(x => x.UpgradeRank == rank);
    }
}

[System.Serializable]
public struct CampUpgradeRecruiting
{
    public int UpgradeRank;
    public int MaxRecruitLevel;
}
