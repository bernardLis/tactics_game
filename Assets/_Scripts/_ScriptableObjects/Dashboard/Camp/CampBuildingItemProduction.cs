using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Item Production")]
public class CampBuildingItemProduction : CampBuilding
{
    public List<CampUpgradeItemProduction> Upgrades = new();

    public override int GetMaxUpgradeRank()
    {
        return Upgrades.OrderByDescending(x => x.UpgradeRank).First().UpgradeRank;
    }

    public CampUpgradeItemProduction GetUpgradeByRank(int rank)
    {
        return Upgrades.FirstOrDefault(x => x.UpgradeRank == rank);
    }

    public void Produce()
    {
        float chance = GetUpgradeByRank(UpgradeRank).DailyChanceToProduceItem;
        if (Random.value < chance)
        {
            Item i = _gameManager.GameDatabase.GetRandomItem();
            Report r = ScriptableObject.CreateInstance<Report>();
            r.Initialize(ReportType.Item, null, null, null, null, null, null, null, null, i);
            _gameManager.AddNewReport(r);
        }
    }
}

[System.Serializable]
public struct CampUpgradeItemProduction
{
    public int UpgradeRank;
    public float DailyChanceToProduceItem;
}
