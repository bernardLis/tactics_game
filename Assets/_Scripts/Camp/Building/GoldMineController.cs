using Lis.Core;
using Lis.Units.Hero;

namespace Lis.Camp.Building
{
    public class GoldMineController : BuildingController, IInteractable
    {
        GoldMine _goldMine;
        public new string InteractionPrompt => "Gold Mine";

        protected override void Initialize()
        {
            Building = GameManager.Campaign.GoldMine;
            _goldMine = (GoldMine)Building;

            base.Initialize();
        }

        public override bool Interact(Interactor interactor)
        {
            return true;
        }
    }
}