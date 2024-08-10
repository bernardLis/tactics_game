using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class ArchitectController : BuildingController, IInteractable
    {
        Architect _architect;
        public new string InteractionPrompt => "Access Architect";

        protected override void Initialize()
        {
            base.Initialize();
            Building = GameManager.Campaign.Architect;
            _architect = (Architect)Building;
        }


        public override bool Interact(Interactor interactor)
        {
            ArchitectScreen architectScreen = new();
            architectScreen.InitializeBuilding(_architect);
            return true;
        }
    }
}