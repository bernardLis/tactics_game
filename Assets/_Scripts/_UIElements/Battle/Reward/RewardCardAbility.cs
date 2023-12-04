using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardCardAbility : RewardCard
{
    public RewardCardAbility(Reward reward) : base(reward)
    {
        RewardAbility rewardAbility = reward as RewardAbility;
        Label txt = new Label("");
        txt.text = $"Level {rewardAbility.Level}";
        Add(txt);
        if (!rewardAbility.IsUpgrade)
        {
            txt.text = "New!";
            txt.style.color = Color.yellow;
        }

        Add(new AbilityButton(rewardAbility.Ability));

        // TODO: handle upgrade and new ability differently.


    }
}
