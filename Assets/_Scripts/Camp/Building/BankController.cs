using Lis.Core;
using Lis.Units.Hero;

namespace Lis.Camp.Building
{
    public class BankController : BuildingController, IInteractable
    {
        Bank _bank;
        public new string InteractionPrompt => "Access Bank";

        protected override void Initialize()
        {
            Building = GameManager.Campaign.Bank;
            _bank = (Bank)Building;

            base.Initialize();
        }

        public override bool Interact(Interactor interactor)
        {
            BankScreen bankScreen = new();
            bankScreen.InitializeBank(_bank);
            return true;
        }
    }
}