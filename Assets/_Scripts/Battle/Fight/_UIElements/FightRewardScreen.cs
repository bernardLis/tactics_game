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
        }

        protected override RewardElement ChooseRewardElement()
        {
            int v = Random.Range(0, 2);
            if (v == 1) return CreateRewardCardPawn();
            return CreateRewardCardCreature();
        }
    }
}