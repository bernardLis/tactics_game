using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Reward Army")]
public class RewardArmy : Reward
{
    float _countPerLevelMultiplier = 0.2f;
    public ArmyGroup ArmyGroup { get; private set; }
    public override void CreateRandom(Hero hero)
    {
        base.CreateRandom(hero);

        // TODO: no schema for upgrading army entities, 
        // so for now just create a new one and add it to hero army
        ArmyGroup = ScriptableObject.CreateInstance<ArmyGroup>();
        ArmyGroup.ArmyEntity = _gameManager.HeroDatabase.GetRandomArmyEntity();

        ArmyGroup.EntityCount = Mathf.RoundToInt(Random.Range(1, 10) * _countPerLevelMultiplier * _hero.Level.Value);
    }

    public override void GetReward()
    {
        base.GetReward();

        _hero.AddArmy(ArmyGroup);
    }
}
