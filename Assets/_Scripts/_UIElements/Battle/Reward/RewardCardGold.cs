using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardCardGold : RewardCard
{
    public RewardCardGold(Reward reward) : base(reward)
    {
        RewardGold rewardGold = reward as RewardGold;
        Add(new GoldElement(rewardGold.Gold));

    }
}
