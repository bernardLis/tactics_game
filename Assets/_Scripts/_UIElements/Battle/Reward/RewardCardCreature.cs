using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardCardCreature : RewardCard
{
    public RewardCardCreature(Reward reward) : base(reward)
    {
        RewardCreature rewardCreature = reward as RewardCreature;
        EntityIcon icon = new(rewardCreature.Creature);
        Add(icon);
    }
}
