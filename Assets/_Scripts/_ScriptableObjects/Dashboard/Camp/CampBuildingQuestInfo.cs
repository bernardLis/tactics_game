using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Quest Info")]
public class CampBuildingQuestInfo : CampBuilding
{
    public List<CampUpgradeQuestInfo> Upgrades = new();

    public override int GetMaxUpgradeRank()
    {
        return Upgrades.OrderByDescending(x => x.UpgradeRank).First().UpgradeRank;
    }

    public CampUpgradeQuestInfo GetUpgradeByRank(int rank)
    {
        return Upgrades.FirstOrDefault(x => x.UpgradeRank == rank);
    }
}

[System.Serializable]
public struct CampUpgradeQuestInfo
{
    public int UpgradeRank;
    public string RevealedInfo;
}
