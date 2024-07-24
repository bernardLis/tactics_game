using Lis.Arena.Fight;
using Lis.Core;
using Lis.Units;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class FightStarter : BuildingController, IInteractable
    {
        public new string InteractionPrompt => "Start Next Fight";

        public override bool Interact(Interactor interactor)
        {
            if (FightManager.IsFightActive)
            {
                Debug.Log("Fight is already active");
                return false;
            }

            foreach (UnitController uc in FightManager.Instance.PlayerUnits)
                if (uc is PlayerUnitController puc)
                    puc.TeleportToArena();

            FightManager.StartFight();
            return true;
        }


        protected override void OnArenaInitialized()
        {
            base.OnArenaInitialized();
            Building = FightManager.Campaign.FightStarter;
            Initialize();
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