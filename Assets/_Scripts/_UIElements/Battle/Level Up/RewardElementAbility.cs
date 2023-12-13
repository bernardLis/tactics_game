using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardElementAbility : RewardElement
{
    public RewardElementAbility(Reward reward) : base(reward)
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

        Add(new AbilityElement(rewardAbility.Ability));

        Label name = new Label(Helpers.ParseScriptableObjectName(rewardAbility.Ability.name));
        name.style.whiteSpace = WhiteSpace.Normal;
        name.style.unityFontStyleAndWeight = FontStyle.Bold;
        Add(name);

        Add(new Label(rewardAbility.Ability.Description));

        // TODO: handle upgrade and new ability differently.


    }
}
