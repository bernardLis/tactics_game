using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Units.Hero.Rewards
{
    public class RewardElementPawn : RewardElement
    {
        public RewardElementPawn(Reward reward) : base(reward)
        {
            RewardPawn rewardPawn = reward as RewardPawn;
            if (rewardPawn == null) return;

            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            ContentContainer.Add(container);

            Label count = new(rewardPawn.Count + "x");
            container.Add(count);

            Label nameLabel = new(rewardPawn.Pawn.UnitName);
            nameLabel.style.whiteSpace = WhiteSpace.Normal;
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            container.Add(nameLabel);

            UnitIcon icon = new(rewardPawn.Pawn);
            ContentContainer.Add(icon);
        }
    }
}