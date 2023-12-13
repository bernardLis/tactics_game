using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardElementGold : RewardElement
{
    public RewardElementGold(Reward reward) : base(reward)
    {
        RewardGold rewardGold = reward as RewardGold;
        Add(new GoldElement(rewardGold.Gold));

    }
}
