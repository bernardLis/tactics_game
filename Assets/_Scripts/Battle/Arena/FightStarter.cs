using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class FightStarter : BuildingController, IInteractable
    {
        public new string InteractionPrompt => "Press F To Start A Fight!";

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


        protected override void OnBattleInitialized()
        {
            base.OnBattleInitialized();
            Building = BattleManager.Battle.FightStarter;
            Initialize();
        }

        protected override void SetTooltipText()
        {
            TooltipText.text = InteractionPrompt;
        }

        void OnFightOptionChosen(FightOption _)
        {
            AllowInteraction();
        }

        protected override void OnFightEnded()
        {
            base.OnFightEnded();
            if (FightManager.FightNumber == 1) Building.Unlock();
            if (!Building.IsUnlocked) return;
            if (FightManager.LastFight != null)
                FightManager.LastFight.OnOptionChosen -= OnFightOptionChosen;
            if (FightManager.CurrentFight != null)
                FightManager.CurrentFight.OnOptionChosen += OnFightOptionChosen;
        }

        protected override void OnFightStarted()
        {
            ForbidInteraction();
        }
    }
}