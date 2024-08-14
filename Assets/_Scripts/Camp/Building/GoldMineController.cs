using Lis.Core;
using Lis.Units;
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

            if (!_goldMine.IsUnlocked) return;

            InitializeGoldMine();
        }

        protected override void Unlock()
        {
            base.Unlock();
            InitializeGoldMine();
        }

        void InitializeGoldMine()
        {
            GetComponentInChildren<UnitDropZoneController>().Initialize(this);

            foreach (Unit u in _goldMine.GetAssignedUnits())
            {
                UnitCampController ucc = CampManager.SpawnUnit(u, transform.position);
                AssignUnit(ucc);
                ucc.StartGoldMineCoroutine(this);
            }
        }

        public override bool CanInteract()
        {
            if (_goldMine.GoldAvailable <= 0) return false;
            return base.CanInteract();
        }

        public override bool Interact(Interactor interactor)
        {
            CampConsoleManager.ShowMessage($"Collected {_goldMine.GoldAvailable} gold from Gold Mine.");
            _goldMine.CollectGold();
            return true;
        }

        public override void AssignUnit(UnitCampController ucc)
        {
            base.AssignUnit(ucc);
            if (_goldMine.GetAssignedUnitCount() >= 2)
            {
                CampConsoleManager.ShowMessage($"Gold Mine is full.");
                return;
            }

            CampConsoleManager.ShowMessage($"Unit assigned to Gold Mine.");
            _goldMine.AssignUnit(ucc.Unit);
            ucc.StartGoldMineCoroutine(this);
        }
    }
}