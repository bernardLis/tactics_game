using Lis.Units.Hero.Rewards;

namespace Lis.Camp.Building
{
    public class LevelUpRewardScreen : RewardScreen
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
            return CreateRewardCardAbility();
        }
    }
}