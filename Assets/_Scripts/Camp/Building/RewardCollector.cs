using Lis.Arena;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class RewardCollector : BuildingController, IInteractable
    {
        int _rewardsAvailable;
        public new string InteractionPrompt => "Collect Reward";


        protected override void Initialize()
        {
            base.Initialize();
            _rewardsAvailable = 0;
            Building = GameManager.Instance.Campaign.RewardCollector;
        }

        public override bool Interact(Interactor interactor)
        {
            if (!IsInteractionAvailable)
            {
                Debug.Log("No reward to collect");
                return false;
            }

            FightRewardScreen fightRewardScreen = new();
            fightRewardScreen.Initialize();
            _rewardsAvailable--;
            if (_rewardsAvailable == 0)
                ForbidInteraction();
            return true;
        }
    }
}