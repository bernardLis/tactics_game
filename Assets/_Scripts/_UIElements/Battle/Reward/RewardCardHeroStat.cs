using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardCardHeroStat : RewardCard
{
    public RewardCardHeroStat(Reward reward) : base(reward)
    {
        RewardHeroStat rewardStat = reward as RewardHeroStat;

        VisualElement icon = new();
        icon.style.width = 100;
        icon.style.height = 100;
        icon.style.backgroundImage = GameManager.Instance.EntityDatabase.GetStatIconByType(rewardStat.StatType).texture;
        Add(icon);

        Label txt = new(rewardStat.StatType.ToString());
        txt.text += $" +{rewardStat.Amount}";
        txt.style.unityFontStyleAndWeight = FontStyle.Bold;
        Add(txt);
    }
}
