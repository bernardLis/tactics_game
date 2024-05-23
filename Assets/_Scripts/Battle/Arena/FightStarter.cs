using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class FightStarter : ArenaInteractable, IInteractable
    {
        public new string InteractionPrompt => "Press F To Start A Fight!";


        protected override void OnBattleInitialized()
        {
            base.OnBattleInitialized();
            FightManager.Instance.OnInitialized += OnFightManagerInitialized;
        }

        void OnFightManagerInitialized()
        {
            FightManager.CurrentFight.OnOptionChosen += OnFightOptionChosen;
        }

        protected override void SetTooltipText()
        {
            TooltipText.text = InteractionPrompt;
        }

        void OnFightOptionChosen(FightOption _)
        {
            InteractionAvailableEffect.SetActive(true);
            IsInteractionAvailable = true;
        }

        protected override void OnFightEnded()
        {
            base.OnFightEnded();
            FightManager.LastFight.OnOptionChosen -= OnFightOptionChosen;
            FightManager.CurrentFight.OnOptionChosen += OnFightOptionChosen;
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