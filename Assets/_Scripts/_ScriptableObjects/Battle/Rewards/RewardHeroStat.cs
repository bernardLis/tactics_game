using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Reward Hero Stat")]
public class RewardHeroStat : Reward
{
    public StatType StatType;
    public int Amount;

    public override bool CreateRandom(Hero hero, List<RewardCard> otherRewardCards)
    {
        base.CreateRandom(hero, otherRewardCards);

        List<Stat> heroStats = hero.GetAllStats();
        List<Stat> availableStatTypes = new();
        foreach (Stat stat in heroStats)
            if (stat.BaseValue < stat.MinMaxValue.y)
                availableStatTypes.Add(stat);

        foreach (RewardCard rc in otherRewardCards)
        {
            if (rc is not RewardCardHeroStat) continue;
            RewardHeroStat rhs = (RewardHeroStat)rc.Reward;
            availableStatTypes.RemoveAll(x => x.StatType == rhs.StatType);
        }

        if (availableStatTypes.Count == 0)
        {
            Debug.LogError("Reward - no stats to upgrade");
            return false;
        }
        Stat selected = availableStatTypes[Random.Range(0, availableStatTypes.Count)];
        StatType = selected.StatType;
        Amount = Random.Range(selected.GrowthPerLevelRange.x, selected.GrowthPerLevelRange.y);

        return true;
    }


    public override void GetReward()
    {
        base.GetReward();

        List<Stat> stats = _hero.GetAllStats();
        foreach (Stat s in stats)
            if (s.StatType == StatType)
                s.ApplyBaseValueChange(Amount);
    }
}
