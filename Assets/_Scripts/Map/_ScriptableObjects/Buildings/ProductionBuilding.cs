using System;
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

    public event Action<int> OnProduced;

    public override void Build()
    {
        base.Build();
        Produce(Mathf.CeilToInt(PerWeekProductionCount * 0.5f));
    }

    public override void Reset()
    {
        base.Reset();
        AvailableToBuyCount = 0;
    }

    public override void OnDayPassed(int day)
    {
        base.OnDayPassed(day);
        if (day % 7 == 0)
            Produce(PerWeekProductionCount);
    }

    public override void Produce(int count)
    {
        if (!IsBuilt) return;
        base.Produce(count);
        AvailableToBuyCount += count;
        OnProduced?.Invoke(AvailableToBuyCount);
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

    public override string GetDescription() { return $"Produces {PerWeekProductionCount} of {ArmyEntity.name} per week."; }

}
