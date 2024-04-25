using Lis.Battle.Fight;
using UnityEngine;

namespace Lis
{
    public class RewardCollector : ArenaInteractable, IInteractable
    {
        public new string InteractionPrompt => "Press F To Collect Reward!";

        protected override void SetTooltipText()
        {
            TooltipText.text = InteractionPrompt;
        }

        protected override void OnFightEnded()
        {
            IsInteractionAvailable = true;
            InteractionAvailableEffect.SetActive(true);
        }

        public override bool Interact(Interactor interactor)
        {
            if (!IsInteractionAvailable)
            {
                Debug.Log("No reward to collect");
                return false;
            }

            FightRewardScreen unused = new();
            InteractionAvailableEffect.SetActive(false);
            IsInteractionAvailable = false;
            return true;
        }
    }
}