using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Better Quests")]
public class CampBuildingQuests : CampBuilding
{
    public List<CampUpgradeQuests> Upgrades = new();

    public CampUpgradeQuests GetUpgradeByRank(int rank)
    {
        return Upgrades.FirstOrDefault(x => x.UpgradeRank == rank);
    }

}

[System.Serializable]
public struct CampUpgradeQuests
{
    public int UpgradeRank;
    public int MaxQuestRank;
}

