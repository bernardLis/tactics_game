using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class BankController : BuildingController, IInteractable
    {
        public new string InteractionPrompt => "Press F To Access Bank!";
        Bank _bank;

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

        protected override void SetTooltipText()
        {
            TooltipText.text = InteractionPrompt;
        }

        public override bool Interact(Interactor interactor)
        {
            if (FightManager.IsFightActive)
            {
                Debug.Log("Fight instead of shopping!");
                return false;
            }

            BankScreen bankScreen = new BankScreen();
            bankScreen.InitializeBank(_bank);
            return true;
        }
    }
}