using System.Collections.Generic;
using Lis.Units.Hero.Rewards;
using UnityEngine;

namespace Lis.Map.MapNodes
{
    [CreateAssetMenu(menuName = "ScriptableObject/Map/Map Node Chest")]
    public class MapNodeChest : MapNode
    {
        int _numberOfRewards;
        readonly List<Reward> _rewards = new();

        public override void Initialize(Vector3 pos, int row)
        {
            base.Initialize(pos, row);
            _numberOfRewards = GameManager.UpgradeBoard.GetUpgradeByName("Reward Count").GetCurrentLevel()
                .Value;
            SelectItems();
        }

        public void SelectItems()
        {
            _rewards.Clear();

            for (int i = 0; i < _numberOfRewards; i++)
            {
                Reward reward = CreateInstance<RewardArmor>();
                reward.CreateRandom(GameManager.Campaign.Hero, _rewards);
                _rewards.Add(reward);
            }
        }

        public List<Reward> GetRewards()
        {
            return _rewards;
        }
    }
}