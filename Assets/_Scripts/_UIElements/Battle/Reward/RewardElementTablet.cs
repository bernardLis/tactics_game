using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardElementTablet : RewardElement
{
    public RewardElementTablet(Reward reward) : base(reward)
    {
        RewardTablet rewardTablet = reward as RewardTablet;

        VisualElement icon = new();
        icon.style.width = 100;
        icon.style.height = 100;
        icon.style.backgroundImage = rewardTablet.Tablet.Icon.texture;
        Add(icon);
    }
}
