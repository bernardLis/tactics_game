using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Troops Limit")]
public class CampBuildingTroopsLimit : CampBuilding
{
    public List<CampUpgradeTroopsLimit> Upgrades = new();

    public override int GetMaxUpgradeRank()
    {
        return Upgrades.OrderByDescending(x => x.UpgradeRank).First().UpgradeRank;
    }

    public CampUpgradeTroopsLimit GetUpgradeByRank(int rank)
    {
        return Upgrades.FirstOrDefault(x => x.UpgradeRank == rank);
    }

    public override void FinishBuilding() { base.FinishBuilding(); }

    public override void Upgrade()
    {
        base.Upgrade();
        _gameManager.ChangeTroopsLimit(GetUpgradeByRank(UpgradeRank).LimitIncrease);
    }
}

[System.Serializable]
public struct CampUpgradeTroopsLimit
{
    public int UpgradeRank;
    public int LimitIncrease;
}
