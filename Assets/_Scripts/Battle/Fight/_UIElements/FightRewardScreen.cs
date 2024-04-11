using Lis.Units.Hero.Rewards;

namespace Lis.Battle.Fight
{
    public class FightRewardScreen : RewardScreen
    {
        protected override RewardElement ChooseRewardElement()
        {
            return CreateRewardCardCreature();
        }
    }
}