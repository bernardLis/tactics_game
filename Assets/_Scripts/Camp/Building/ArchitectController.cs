using Lis.Arena.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class ArchitectController : BuildingController, IInteractable
    {
        Architect _architect;
        public new string InteractionPrompt => "Access Architect";


        protected override void OnArenaInitialized()
        {
            base.OnArenaInitialized();
            Building = FightManager.Campaign.Architect;
            _architect = (Architect)Building;
            OnFightEnded();
            Initialize();
        }

        protected override void OnFightEnded()
        {
            if (!_architect.IsUnlocked) return;
            AllowInteraction();
        }

        protected override void OnFightStarted()
        {
            if (!_architect.IsUnlocked) return;
            ForbidInteraction();
        }

        public override bool Interact(Interactor interactor)
        {
            if (FightManager.IsFightActive)
            {
                Debug.Log("Fight instead of shopping!");
                return false;
            }

            ArchitectScreen architectScreen = new();
            architectScreen.InitializeBuilding(_architect);
            return true;
        }
    }
}