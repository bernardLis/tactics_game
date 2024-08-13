using System.Collections.Generic;
using Lis.Core;
using Lis.Units;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class GoldMineController : BuildingController, IInteractable
    {
        GoldMine _goldMine;
        public new string InteractionPrompt => "Gold Mine";

        readonly List<UnitCampController> _assignedUnits = new();

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
            GetComponentInChildren<UnitReleaseController>().OnUnitsReleased += ReleaseUnits;

            foreach (Unit u in _goldMine.GetAssignedUnits())
            {
                UnitCampController ucc = CampManager.SpawnUnit(u, transform.position);
                AssignUnit(ucc);
                ucc.StartGoldMineCoroutine(this);
            }
        }

        public override bool Interact(Interactor interactor)
        {
            Debug.Log("Interacting with Gold Mine");
            _goldMine.CollectGold();
            return true;
        }

        public override void AssignUnit(UnitCampController ucc)
        {
            base.AssignUnit(ucc);
            if (_goldMine.GetAssignedUnitCount() >= 2)
            {
                Debug.Log("2 peasants are already assigned to the Gold Mine");
                return;
            }

            _assignedUnits.Add(ucc);
            _goldMine.AssignUnit(ucc.Unit);
            ucc.StartGoldMineCoroutine(this);
        }

        void ReleaseUnits()
        {
            foreach (UnitCampController ucc in _assignedUnits)
            {
                ucc.ReleaseFromBuildingAssignment();
            }

            _goldMine.ReleaseUnits();
        }
    }
}