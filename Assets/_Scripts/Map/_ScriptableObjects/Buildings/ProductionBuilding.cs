using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Production Building")]
public class ProductionBuilding : Building
{
    public ArmyEntity ArmyEntity;
    public int PricePerEntity;
    
    public int PerWeekProductionCount;
    public int AvailableToBuyCount;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void OnDayPassed(int day)
    {
        base.OnDayPassed(day);
        if (day % 7 == 0)
            Produce();
    }

    public override void Produce()
    {
        base.Produce();
        AvailableToBuyCount += PerWeekProductionCount;
    }

    public override BuildingData SerializeSelf()
    {
        BuildingData data = base.SerializeSelf();

        data.AvailableToBuyCount = AvailableToBuyCount;
        return data;
    }

    public override void LoadFromData(BuildingData data)
    {
        base.LoadFromData(data);
        AvailableToBuyCount = data.AvailableToBuyCount;
    }
}
