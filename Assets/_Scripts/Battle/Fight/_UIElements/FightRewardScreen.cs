using Lis.Battle.Arena;
using Lis.Units.Hero.Rewards;
using UnityEngine;

namespace Lis.Battle.Fight
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