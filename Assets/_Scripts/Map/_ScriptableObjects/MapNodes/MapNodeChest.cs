using System.Collections.Generic;
using Lis.Units.Hero.Rewards;
using UnityEngine;

namespace Lis.Map.MapNodes
{
    [CreateAssetMenu(menuName = "ScriptableObject/Map/Map Node Chest")]
    public class MapNodeChest : MapNode
    {
        readonly List<Reward> _rewards = new();

        public override void Initialize(Vector3 pos, int row)
        {
            base.Initialize(pos, row);
            for (int i = 0; i < 3; i++)
            {
                Reward reward = CreateInstance<RewardArmor>();
                _rewards.Add(reward);
            }
        }

        public List<Reward> GetRewards()
        {
            return _rewards;
        }

        public void SetRewards(List<Reward> rewards)
        {
            _rewards.Clear();
            _rewards.AddRange(rewards);
        }
    }
}