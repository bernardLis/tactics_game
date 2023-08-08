using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardCardAbility : RewardCard
{
    public RewardCardAbility(Reward reward) : base(reward)
    {
        RewardAbility rewardAbility = reward as RewardAbility;
        if (rewardAbility.IsUpgrade)
            Add(new Label("Upgrade"));
        else
            Add(new Label("New Ability"));
        Add(new AbilityButton(rewardAbility.Ability));

        // TODO: handle upgrade and new ability differently.


    }
}
