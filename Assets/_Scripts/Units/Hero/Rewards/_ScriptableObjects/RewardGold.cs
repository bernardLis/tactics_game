using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Lis.Units.Hero.Rewards
{
    public class RewardGold : Reward
    {
        public int Gold { get; private set; }

        public override bool CreateRandom(Hero hero, List<Reward> otherRewards)
        {
            base.CreateRandom(hero, otherRewards);
            Gold = Random.Range(100, 200);
            SetPrice();
            return true;
        }

        protected override void SetPrice()
        {
            Price = Gold;
        }

        public override void GetReward()
        {
            base.GetReward();
            GameManager.ChangeGoldValue(Gold);
        }
    }
}