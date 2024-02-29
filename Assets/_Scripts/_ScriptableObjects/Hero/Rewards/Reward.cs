using System;
using System.Collections.Generic;
using Lis.Core;


namespace Lis
{
    public class Reward : BaseScriptableObject
    {
        protected GameManager _gameManager;

        protected Hero _hero;

        public event Action<Reward> OnRewardSelected;
        public virtual bool CreateRandom(Hero hero, List<RewardElement> otherRewardCards)
        {
            _gameManager = GameManager.Instance;
            _hero = hero;

            return true;
        }

        public virtual void GetReward()
        {
            OnRewardSelected?.Invoke(this);
        }
    }
}
