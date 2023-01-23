using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Troops Limit")]
public class CampBuildingTroopsLimit : CampBuilding
{

    public List<CampTroopsLimitUpgrade> CampTroopsLimitUpgrades = new();


    public CampTroopsLimitUpgrade GetTroopsLimitIncreaseByRank(int rank)
    {
        return CampTroopsLimitUpgrades.FirstOrDefault(x => x.UpgradeRank == rank);
    }

    public override void FinishBuilding() { base.FinishBuilding(); }

    public override void Upgrade()
    {
        base.Upgrade();
        _gameManager.ChangeTroopsLimit(GetTroopsLimitIncreaseByRank(UpgradeRank).LimitIncrease);
    }
}

[System.Serializable]
public struct CampTroopsLimitUpgrade
{
    public int UpgradeRank;
    public int LimitIncrease;
}
