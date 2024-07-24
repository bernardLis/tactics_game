using Lis.Camp.Building;
using Lis.Units.Hero.Rewards;

namespace Lis.Arena.Fight
{
    public class FightRewardScreen : RewardScreen
    {
        public override void Initialize()
        {
            base.Initialize();
            MakeItRain();
            PlayLevelUpAnimation();
            SetTitle("Choose A Reward:");
        }

        protected override RewardElement ChooseRewardElement()
        {
            return CreateRewardCardArmor();
        }
    }
}