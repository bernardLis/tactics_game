using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Arena.Building
{
    public class BankController : BuildingController, IInteractable
    {
        Bank _bank;
        public new string InteractionPrompt => "Access Bank";

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

        protected override void OnBattleInitialized()
        {
            base.OnBattleInitialized();
            Building = BattleManager.Battle.Bank;
            _bank = (Bank)Building;
            _bank.InitializeBattle();
            OnFightEnded();
            Initialize();
        }

        protected override void OnFightEnded()
        {
            if (!_bank.IsUnlocked) return;
            AllowInteraction();
        }

        protected override void OnFightStarted()
        {
            if (!_bank.IsUnlocked) return;
            ForbidInteraction();
        }
    }
}