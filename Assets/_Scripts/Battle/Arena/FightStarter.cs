using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class FightStarter : BuildingController, IInteractable
    {
        public new string InteractionPrompt => "Press F To Start A Fight!";


        protected override void OnBattleInitialized()
        {
            base.OnBattleInitialized();
            Building = BattleManager.Battle.FightStarter;
            FightManager.Instance.OnInitialized += OnFightManagerInitialized;
            Initialize();
        }

        void OnFightManagerInitialized()
        {
            FightManager.CurrentFight.OnOptionChosen += OnFightOptionChosen;
        }

        protected override void SetTooltipText()
        {
            TooltipText.text = InteractionPrompt;
        }

        void OnFightOptionChosen(FightOption _)
        {
            AllowInteraction();
        }

        protected override void OnFightEnded()
        {
            base.OnFightEnded();
            FightManager.LastFight.OnOptionChosen -= OnFightOptionChosen;
            FightManager.CurrentFight.OnOptionChosen += OnFightOptionChosen;
        }

        protected override void OnFightStarted()
        {
            ForbidInteraction();
        }

        public override bool Interact(Interactor interactor)
        {
            if (FightManager.IsFightActive)
            {
                Debug.Log("Fight is already active");
                return false;
            }

            FightManager.StartFight();
            return true;
        }
    }
}