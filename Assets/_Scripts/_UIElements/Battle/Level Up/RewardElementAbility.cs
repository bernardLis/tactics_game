using Lis.Core.Utilities;
using Lis.Units.Hero.Ability;
using Lis.Units.Hero.Rewards;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class RewardElementAbility : RewardElement
    {
        public RewardElementAbility(Reward reward) : base(reward)
        {
            RewardAbility rewardAbility = reward as RewardAbility;
            if (rewardAbility == null) return;

            Label txt = new($"Level {rewardAbility.Level}");
            Add(txt);

            if (!rewardAbility.IsUpgrade)
            {
                txt.text = "New!";
                txt.style.color = rewardAbility.Ability.Element.Color.Primary;
            }

            Add(new Element(rewardAbility.Ability, size: 200));

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