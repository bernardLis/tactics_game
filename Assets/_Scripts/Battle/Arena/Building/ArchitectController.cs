using Lis.Battle.Arena;
using Lis.Battle.Arena.Building;
using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis
{
    public class ArchitectController : BuildingController, IInteractable
    {
        Architect _architect;
        public new string InteractionPrompt => "Access Architect";


        protected override void OnBattleInitialized()
        {
            base.OnBattleInitialized();
            Building = BattleManager.Battle.Architect;
            _architect = (Architect)Building;
            OnFightEnded();
            Initialize();
        }

        protected override void OnFightEnded()
        {
            if (FightManager.FightNumber == 6)
                Building.Unlock();

            if (!_architect.IsUnlocked) return;
            AllowInteraction();
        }

        protected override void OnFightStarted()
        {
            if (!_architect.IsUnlocked) return;
            ForbidInteraction();
        }

        public override bool Interact(Interactor interactor)
        {
            if (FightManager.IsFightActive)
            {
                Debug.Log("Fight instead of shopping!");
                return false;
            }

            ArchitectScreen architectScreen = new();
            architectScreen.InitializeBuilding(_architect);
            return true;
        }
    }
}