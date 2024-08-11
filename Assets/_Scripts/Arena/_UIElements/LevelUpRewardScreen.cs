using Lis.Core;
using Lis.Units.Hero.Rewards;

namespace Lis.Arena
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

    }
}