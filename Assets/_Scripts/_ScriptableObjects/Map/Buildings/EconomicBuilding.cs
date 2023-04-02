using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Economic Building")]
public class EconomicBuilding : Building
{
    public int GoldPerDay;

    public override void OnDayPassed(int day)
    {
        base.OnDayPassed(day);
        Produce(GoldPerDay);
    }

    public override void Produce(int count)
    {
        if (!IsBuilt) return;
        base.Produce(count);
        _gameManager.ChangeGoldValue(count);
    }

    public override string GetDescription() { return $"Produces {GoldPerDay} gold per day."; }
}
