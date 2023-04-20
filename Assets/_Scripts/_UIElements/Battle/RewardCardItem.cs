using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardCardItem : RewardCard
{
    public RewardCardItem(Reward reward) : base(reward)
    {
        RewardItem rewardItem = reward as RewardItem;
        Add(new ItemElement(rewardItem.Item));
    }
}
