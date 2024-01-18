using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace Lis
{
    public class RewardGold : Reward
    {
        public int Gold { get; private set; }

        public override bool CreateRandom(Hero hero, List<RewardElement> otherRewardCards)
        {
            base.CreateRandom(hero, otherRewardCards);
            Gold = Random.Range(100, 200);
            return true;
        }

        public override void GetReward()
        {
            base.GetReward();
            _gameManager.ChangeGoldValue(Gold);
        }
    }
}
