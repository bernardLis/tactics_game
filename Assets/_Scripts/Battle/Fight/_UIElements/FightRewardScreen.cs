using Lis.Units.Hero.Rewards;

namespace Lis.Battle.Fight
{
    public class FightRewardScreen : RewardScreen
    {
        public FightRewardScreen()
        {
            MakeItRain();
            PlayLevelUpAnimation();
        }

        protected override RewardElement ChooseRewardElement()
        {
            return CreateRewardCardCreature();
        }
    }
}