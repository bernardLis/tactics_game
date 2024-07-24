using Lis.Arena.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class FightSelector : BuildingController, IInteractable
    {
        public new string InteractionPrompt => "Select A Fight";

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

        protected override void OnArenaInitialized()
        {
            base.OnArenaInitialized();
            Building = FightManager.Campaign.FightSelector;
            Initialize();
        }

        protected override void OnFightEnded()
        {
            if (FightManager.FightNumber == 5)
                Building.Unlock();

            if (Building.IsUnlocked)
                AllowInteraction();
        }

        protected override void OnFightStarted()
        {
            ForbidInteraction();
        }
    }
}