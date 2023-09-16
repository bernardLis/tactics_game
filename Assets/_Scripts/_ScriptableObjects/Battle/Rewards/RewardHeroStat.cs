using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Reward Hero Stat")]
public class RewardHeroStat : Reward
{
    public StatType StatType;
    public int Amount;

    public override void CreateRandom(Hero hero)
    {
        base.CreateRandom(hero);
        List<StatType> statTypes = new List<StatType>
        {
            StatType.Health,
            StatType.Speed
        };

        // hero max speed 20
        if (_gameManager.PlayerHero.Speed.BaseValue == 20)
            statTypes.Remove(StatType.Speed);

        StatType = statTypes[Random.Range(0, statTypes.Count)];
        if (StatType == StatType.Health)
            Amount = 10;
        if (StatType == StatType.Speed)
            Amount = 1;
    }

    public override void GetReward()
    {
        base.GetReward();
        if (StatType == StatType.Health)
        {
            _gameManager.PlayerHero.MaxHealth.ApplyBaseValueChange(Amount);
            _gameManager.PlayerHero.CurrentHealth.ApplyChange(Amount);
        }
        if (StatType == StatType.Speed)
            _gameManager.PlayerHero.Speed.ApplyBaseValueChange(Amount);
    }
}
