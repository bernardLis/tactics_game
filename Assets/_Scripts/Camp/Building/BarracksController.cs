using Lis.Core;
using Lis.Units.Hero;

namespace Lis.Camp.Building
{
    public class BarracksController : BuildingController, IInteractable
    {
        Barracks _barracks;


        protected override void Initialize()
        {
            base.Initialize();
            _barracks = GameManager.Campaign.Barracks;
            Building = _barracks;
            AllowInteraction();
        }

        public new string InteractionPrompt => "Interact";

        public override bool Interact(Interactor interactor)
        {
            BarracksScreen screen = new();
            screen.InitializeBuilding(_barracks, this);

            return true;
        }
    }
}