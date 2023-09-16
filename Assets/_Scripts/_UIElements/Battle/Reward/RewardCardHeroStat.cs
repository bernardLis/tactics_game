using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardCardHeroStat : RewardCard
{
    public RewardCardHeroStat(Reward reward) : base(reward)
    {
        RewardHeroStat rewardStat = reward as RewardHeroStat;

        Stat stat = ScriptableObject.CreateInstance<Stat>();
        stat.StatType = rewardStat.StatType;
        stat.BaseValue = rewardStat.Amount;
        stat.Initialize();

        StatElement statElement = new(stat);
        Add(statElement);
    }
}
