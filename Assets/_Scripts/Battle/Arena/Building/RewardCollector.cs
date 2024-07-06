using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Arena.Building
{
    public class RewardCollector : BuildingController, IInteractable
    {
        int _rewardsAvailable;
        public new string InteractionPrompt => "Collect Reward";

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

        protected override void OnBattleInitialized()
        {
            base.OnBattleInitialized();
            _rewardsAvailable = 0;
            Building = BattleManager.Battle.RewardCollector;
            Initialize();
        }

        protected override void OnFightEnded()
        {
            if (FightManager.FightNumber == 1) Building.Unlock();
            if (!Building.IsUnlocked) return;
            AllowInteraction();
            _rewardsAvailable++;
        }
    }
}