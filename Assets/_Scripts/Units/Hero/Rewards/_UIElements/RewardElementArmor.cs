using Lis.Units.Hero.Items;

namespace Lis.Units.Hero.Rewards
{
    public class RewardElementArmor : RewardElement
    {
        public RewardElementArmor(Reward reward) : base(reward)
        {
            RewardArmor rewardArmor = reward as RewardArmor;
            if (rewardArmor == null) return;

            ContentContainer.Add(new ArmorElement(rewardArmor.Armor));
        }
    }
}