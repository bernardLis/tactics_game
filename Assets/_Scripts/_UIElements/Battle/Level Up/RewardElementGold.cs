


using Lis.Core;
using Lis.Units.Hero.Rewards;

namespace Lis
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
