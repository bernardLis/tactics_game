using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Minion")]
public class Minion : EntityMovement
{
    public override void InitializeBattle(int team)
    {
        base.InitializeBattle(team);
        if (EntityName.Length == 0) EntityName = Helpers.ParseScriptableObjectName(name);

        BaseTotalHealth.ApplyChange(Random.Range(HealthGrowthPerLevel.x, HealthGrowthPerLevel.y) * Level.Value);
        BaseArmor.ApplyChange(Random.Range(ArmorGrowthPerLevel.x, ArmorGrowthPerLevel.y) * Level.Value);

        BaseSpeed.ApplyChange(Random.Range(SpeedGrowthPerLevel.x, SpeedGrowthPerLevel.y) * Level.Value);
    }
}

