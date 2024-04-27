using Lis.Units.Creature;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Units.Hero.Rewards
{
    public class RewardElementCreature : RewardElement
    {
        public RewardElementCreature(Reward reward) : base(reward)
        {
            RewardCreature rewardCreature = reward as RewardCreature;
            if (rewardCreature == null) return;

            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            ContentContainer.Add(container);

            Label count = new(rewardCreature.Count + "x");
            container.Add(count);

            Label nameLabel = new(rewardCreature.Creature.UnitName);
            nameLabel.style.whiteSpace = WhiteSpace.Normal;
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            container.Add(nameLabel);

            CreatureIcon icon = new(rewardCreature.Creature);
            icon.SetBigIcon();
            ContentContainer.Add(icon);
        }
    }
}