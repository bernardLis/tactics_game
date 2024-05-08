using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class BankDisplayer : ArenaInteractable, IInteractable
    {
        BattleManager _battleManager;
        public new string InteractionPrompt => "Press F To Access Bank!";
        Bank _bank;

        protected override void Start()
        {
            base.Start();

            _battleManager = BattleManager.Instance;
            _battleManager.GetComponent<BattleInitializer>().OnBattleInitialized += OnBattleInitialized;
        }

        void OnBattleInitialized()
        {
            _bank = _battleManager.Battle.Bank;
            _bank.Initialize();
            OnFightEnded();
        }

        protected override void OnFightEnded()
        {
            InteractionAvailableEffect.SetActive(true);
            IsInteractionAvailable = true;
        }

        protected override void OnFightStarted()
        {
            InteractionAvailableEffect.SetActive(false);
            HideTooltip();
            IsInteractionAvailable = false;
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