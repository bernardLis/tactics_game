using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class FightSelector : BuildingController, IInteractable
    {
        public new string InteractionPrompt => "Press F To Select A Fight!";

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

        protected override void OnBattleInitialized()
        {
            base.OnBattleInitialized();
            Building = BattleManager.Battle.FightSelector;
            Initialize();
        }

        protected override void OnFightEnded()
        {
            if (FightManager.FightNumber == 5)
                Building.Unlock();

            if (Building.IsUnlocked)
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
    }
}