using Lis.Core;

namespace Lis.Units.Hero.Rewards
{
    public class RewardElementGold : RewardElement
    {
        public RewardElementGold(Reward reward) : base(reward)
        {
            RewardGold rewardGold = reward as RewardGold;
            Add(new GoldElement(rewardGold.Gold));

        }
    }
}
