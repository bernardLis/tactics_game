using Lis.Arena.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class BankController : BuildingController, IInteractable
    {
        Bank _bank;
        public new string InteractionPrompt => "Access Bank";

        protected override void Initialize()
        {
            base.Initialize();
            Building = GameManager.Campaign.Bank;
            _bank = (Bank)Building;
            AllowInteraction();
        }

        public override bool Interact(Interactor interactor)
        {
            if (FightManager.IsFightActive)
            {
                Debug.Log("Fight instead of shopping!");
                return false;
            }

            BankScreen bankScreen = new();
            bankScreen.InitializeBank(_bank);
            return true;
        }
    }
}