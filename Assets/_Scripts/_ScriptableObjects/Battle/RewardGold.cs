using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Reward Gold")]
public class RewardGold : Reward
{
    float _goldPerLevelMultiplier = 1.1f;

    public int Gold { get; private set; }

    public override void CreateRandom(Hero hero)
    {
        base.CreateRandom(hero);
        Gold = (int)(Random.Range(100, 200) * _goldPerLevelMultiplier * _hero.Level.Value);
    }

    public override void GetReward()
    {
        base.GetReward();
        _gameManager.ChangeGoldValue(Gold);
    }
}
