using Lis.Units.Hero.Rewards;
using UnityEngine;

namespace Lis.Battle.Arena
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
            float v = Random.value;
            RewardElement card = v > 0.5f ? CreateRewardCardAbility() : CreateRewardTablet();

            if (card == null && v > 0.5f) card = CreateRewardTablet();
            if (card == null && v <= 0.5f) card = CreateRewardCardAbility();
            return card;
        }
    }
}