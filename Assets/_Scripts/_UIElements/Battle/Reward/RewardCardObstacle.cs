using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardCardObstacle : RewardCard
{
    public RewardCardObstacle(Reward reward) : base(reward)
    {
        RewardObstacle rewardObstacle = reward as RewardObstacle;
        Label icon = new Label();
        icon.style.width = 100;
        icon.style.height = 100;
        icon.style.backgroundImage = new StyleBackground(rewardObstacle.Icon);
        Add(icon);
        Add(new Label($"Obstacle {rewardObstacle.Size.x}x{rewardObstacle.Size.y}x{rewardObstacle.Size.z}"));
    }
}
