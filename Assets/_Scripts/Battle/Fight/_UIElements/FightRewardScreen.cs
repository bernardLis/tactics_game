using Lis.Battle.Arena;
using Lis.Units.Hero.Rewards;

namespace Lis.Battle.Fight
{
    public class FightRewardScreen : RewardScreen
    {
        public override void Initialize()
        {
            base.Initialize();
            MakeItRain();
            PlayLevelUpAnimation();
        }

        protected override RewardElement ChooseRewardElement()
        {
            return CreateRewardCardCreature();
        }
    }
}