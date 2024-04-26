using Lis.Battle.Fight;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class RewardCollector : ArenaInteractable, IInteractable
    {
        public new string InteractionPrompt => "Press F To Collect Reward!";

        int _rewardsAvailable;

        protected override void SetTooltipText()
        {
            TooltipText.text = InteractionPrompt;
        }

        protected override void OnFightEnded()
        {
            IsInteractionAvailable = true;
            InteractionAvailableEffect.SetActive(true);
            _rewardsAvailable++;
        }

        public override bool Interact(Interactor interactor)
        {
            if (!IsInteractionAvailable)
            {
                Debug.Log("No reward to collect");
                return false;
            }

            FightRewardScreen unused = new();
            _rewardsAvailable--;
            if (_rewardsAvailable == 0)
            {
                InteractionAvailableEffect.SetActive(false);
                IsInteractionAvailable = false;
            }
            return true;
        }
    }
}