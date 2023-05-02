using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardCardArmy : RewardCard
{
    public RewardCardArmy(Reward reward) : base(reward)
    {
        RewardArmy rewardArmy = reward as RewardArmy;

        Add(new ArmyGroupElement(rewardArmy.ArmyGroup));
    }
}
