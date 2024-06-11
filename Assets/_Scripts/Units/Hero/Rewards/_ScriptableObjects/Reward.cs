using System;
using System.Collections.Generic;
using Lis.Core;

namespace Lis.Units.Hero.Rewards
{
    public class Reward : BaseScriptableObject
    {
        public int Price;
        protected GameManager GameManager;
        protected Hero Hero;

        public event Action<Reward> OnRewardSelected;

        public virtual bool CreateRandom(Hero hero, List<RewardElement> otherRewardCards)
        {
            GameManager = GameManager.Instance;
            Hero = hero;

            return true;
        }

        protected virtual void SetPrice()
        {
            Price = 200;
        }

        public virtual void GetReward()
        {
            OnRewardSelected?.Invoke(this);
        }
    }
}