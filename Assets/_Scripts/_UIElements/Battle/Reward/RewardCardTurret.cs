using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardCardTurret : RewardCard
{
    public RewardCardTurret(Reward reward) : base(reward)
    {
        RewardTurret rewardTurret = reward as RewardTurret;
        Label icon = new();
        icon.style.width = 100;
        icon.style.height = 100;
        icon.style.backgroundImage = rewardTurret.Turret.Icon.texture;
        Add(icon);

        Add(new Label($"{rewardTurret.Turret.Element.ElementName} Turret"));
    }
}
