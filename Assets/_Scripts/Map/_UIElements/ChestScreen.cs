using Lis.Core;
using Lis.Core.Utilities;
using Lis.Map.MapNodes;
using UnityEngine;

namespace Lis.Map
{
    public class ChestScreen : RewardScreen
    {
        MapNodeChest _chest;

        public void InitializeChest(MapNodeChest chest)
        {
            _chest = chest;

            Initialize();
        }

        public override void Initialize()
        {
            SetTitle("Chest");
            ParseRewardCards(_chest.GetRewards());
            base.Initialize();
        }

        protected override void RerollReward()
        {
            if (Hero.RewardRerolls <= 0)
            {
                Helpers.DisplayTextOnElement(FightManager.Root, RerollButton, "Not More Rerolls!", Color.red);
                return;
            }

            _chest.SelectItems();

            base.RerollReward();
        }
    }
}