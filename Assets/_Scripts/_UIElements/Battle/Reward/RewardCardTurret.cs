using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardCardTurret : RewardCard
{
    public RewardCardTurret(Reward reward) : base(reward)
    {
        RewardTurret rewardTurret = reward as RewardTurret;
        
        Add(new TurretIcon(rewardTurret.Turret));
        Add(new Label($"{rewardTurret.Turret.Element.ElementName} Turret"));
    }
}
