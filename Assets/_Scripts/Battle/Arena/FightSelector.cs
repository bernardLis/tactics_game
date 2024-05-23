using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class FightSelector : ArenaInteractable, IInteractable
    {
        public new string InteractionPrompt => "Press F To Select A Fight!";

        protected override void OnBattleInitialized()
        {
            Debug.Log("FightSelector initialized!");
            base.OnBattleInitialized();
            OnFightEnded();
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

        protected override void SetTooltipText()
        {
            TooltipText.text = InteractionPrompt;
        }

        public override bool Interact(Interactor interactor)
        {
            if (FightManager.IsFightActive)
            {
                Debug.Log("Fight is active!");
                return false;
            }

            FightSelectScreen fss = new();
            fss.Initialize();
            return true;
        }
    }
}