using System.Collections.Generic;
using Lis.Units.Hero.Rewards;
using UnityEngine;

namespace Lis.Camp.Building
{
    [CreateAssetMenu(menuName = "ScriptableObject/Camp/Building/Shop")]
    public class Shop : Building
    {
        public bool ShouldReset;
        readonly List<Reward> _rewards = new();

        public void SetRewards(List<Reward> newRewards)
        {
            _rewards.Clear();
            _rewards.AddRange(newRewards);
        }

        public List<Reward> GetRewards()
        {
            return _rewards;
        }
    }
}