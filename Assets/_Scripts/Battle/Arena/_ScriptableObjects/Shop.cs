using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero.Rewards;
using UnityEngine;

namespace Lis.Battle.Arena
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Shop")]
    public class Shop : BaseScriptableObject
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