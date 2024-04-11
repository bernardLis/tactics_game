using Lis.Core.Utilities;
using Lis.Units.Hero.Ability;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Units.Hero.Rewards
{
    public class RewardElementAbility : RewardElement
    {
        public RewardElementAbility(Reward reward) : base(reward)
        {
            RewardAbility rewardAbility = reward as RewardAbility;
            if (rewardAbility == null) return;

            Label txt = new($"Level {rewardAbility.Level}");
            Add(txt);

            bool isUpgrade = true;
            if (!rewardAbility.IsUpgrade)
            {
                txt.text = "New!";
                txt.style.color = rewardAbility.Ability.Nature.Color.Primary;
                isUpgrade = false;
            }

            Add(new AbilityElement(rewardAbility.Ability, size: 200, isUpgrade: isUpgrade));

            Label nameLabel = new(Helpers.ParseScriptableObjectName(rewardAbility.Ability.name));
            nameLabel.style.whiteSpace = WhiteSpace.Normal;
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            Add(nameLabel);

            Label descLabel = new(rewardAbility.Ability.Description);
            descLabel.style.width = Length.Percent(50);
            descLabel.style.whiteSpace = WhiteSpace.Normal;
            Add(descLabel);
        }
    }
}