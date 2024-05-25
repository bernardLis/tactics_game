using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class FightSelector : BuildingController, IInteractable
    {
        public new string InteractionPrompt => "Press F To Select A Fight!";

        protected override void OnBattleInitialized()
        {
            base.OnBattleInitialized();
            Building = BattleManager.Battle.FightSelector;
            OnFightEnded();
            Initialize();
        }

        protected override void OnFightEnded()
        {
            AllowInteraction();
        }

        protected override void OnFightStarted()
        {
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
                Debug.Log("Fight is active!");
                return false;
            }

            FightSelectScreen fss = new();
            fss.Initialize();
            return true;
        }
    }
}