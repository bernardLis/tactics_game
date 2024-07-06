using UnityEngine.UIElements;

namespace Lis.Units.Hero.Rewards
{
    public class RewardElementArmor : RewardElement
    {
        public RewardElementArmor(Reward reward) : base(reward)
        {
            RewardArmor rewardArmor = reward as RewardArmor;
            if (rewardArmor == null) return;

            Label name = new(rewardArmor.Armor.name);
            ContentContainer.Add(name);

            // TODO: swap to Item Icon with tooltip and stuff
            VisualElement icon = new();
            icon.style.backgroundImage = new(rewardArmor.Armor.Icon);
            icon.style.width = 100;
            icon.style.height = 100;
            ContentContainer.Add(icon);

            Label stat = new($"StatType: {rewardArmor.Armor.StatType.ToString()} +{rewardArmor.Armor.Value}");
            ContentContainer.Add(stat);
        }
    }
}