using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Economic Building")]
public class EconomicBuilding : Building
{
    public int GoldPerDay;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void OnDayPassed(int day)
    {
        base.OnDayPassed(day);
        Produce();
    }

    public override void Produce()
    {
        base.Produce();
        _gameManager.ChangeGoldValue(GoldPerDay);
    }

}
