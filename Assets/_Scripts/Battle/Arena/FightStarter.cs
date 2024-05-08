using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class FightStarter : ArenaInteractable, IInteractable
    {
        public new string InteractionPrompt => "Press F To Start A Fight!";

        protected override void Start()
        {
            base.Start();
            OnFightEnded();
        }

        protected override void SetTooltipText()
        {
            TooltipText.text = InteractionPrompt;
        }

        protected override void OnFightEnded()
        {
            InteractionAvailableEffect.SetActive(true);
            IsInteractionAvailable = true;
        }

        protected override void OnFightStarted()
        {
            InteractionAvailableEffect.SetActive(false);
            HideTooltip();
            IsInteractionAvailable = false;
        }

        public override bool Interact(Interactor interactor)
        {
            if (FightManager.IsFightActive)
            {
                Debug.Log("Fight is already active");
                return false;
            }

            FightManager.StartFight();
            return true;
        }
    }
}