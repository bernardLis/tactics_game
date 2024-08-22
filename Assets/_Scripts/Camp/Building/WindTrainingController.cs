using System.Collections.Generic;
using Lis.Core;
using Lis.Units;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class WindTrainingController : BuildingController, IInteractable
    {
        [Header("Water Training")]
        [SerializeField] Transform _standPoint;

        [SerializeField] CrateController[] _cratePrefabs;
        [SerializeField] Transform _crateHolder;
        readonly List<CrateController> _crates = new();

        TrainingBuilding _trainingBuilding;
        public new string InteractionPrompt => "Collect Pawns";

        protected override void Initialize()
        {
            Building = GameManager.Campaign.WindTrainingBuilding;
            _trainingBuilding = (TrainingBuilding)Building;

            base.Initialize();

            if (!_trainingBuilding.IsUnlocked) return;

            InitializeFireTrainingBuilding();
        }

        protected override void Unlock()
        {
            base.Unlock();
            InitializeFireTrainingBuilding();
        }

        void InitializeFireTrainingBuilding()
        {
            GetComponentInChildren<UnitDropZoneController>().Initialize(this);

            for (int i = 0; i < _trainingBuilding.GetAssignedWorkers().Count; i++)
            {
                UnitCampController ucc =
                    CampManager.SpawnUnit(_trainingBuilding.GetAssignedWorkers()[i], transform.position);
                SetWorker(ucc);
                ucc.StartHouseCoroutine(_standPoint);
            }

            int cratesToSpawn = _trainingBuilding.AvailablePawnCount;
            cratesToSpawn = Mathf.Clamp(cratesToSpawn, 0, 4);
            for (int i = 0; i < cratesToSpawn; i++)
                SpawnCrate();
        }

        void SpawnCrate()
        {
            CrateController crate =
                Instantiate(_cratePrefabs[Random.Range(0, _cratePrefabs.Length)], _crateHolder);
            crate.transform.localPosition = new(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            crate.gameObject.SetActive(true);
            _crates.Add(crate);
        }

        public override bool CanInteract()
        {
            if (_trainingBuilding.AvailablePawnCount <= 0) return false;
            return base.CanInteract();
        }

        public override bool Interact(Interactor interactor)
        {
            CampConsoleManager.ShowMessage(
                $"Collected {_trainingBuilding.AvailablePawnCount} Pawns from Training Building.");

            for (int i = 0; i < _trainingBuilding.AvailablePawnCount; i++)
            {
                HeroCampController.Instance.DisplayFloatingText($"+{_trainingBuilding.UnitToTrain.UnitName}",
                    Color.black);
                Unit p = Instantiate(_trainingBuilding.UnitToTrain);
                GameManager.Campaign.Hero.AddArmy(Instantiate(p));
                UnitCampController ucc = CampManager.SpawnUnit(p, _crateHolder.position);
                ucc.StartCampCoroutine();
            }

            for (int i = _crates.Count - 1; i >= 0; i--)
                _crates[i].DestroySelf();
            _crates.Clear();

            return true;
        }

        public override void SetWorker(UnitCampController ucc)
        {
            base.SetWorker(ucc);
            CampConsoleManager.ShowMessage($"Unit assigned to Wind Training Building.");
            ucc.StartHouseCoroutine(_standPoint);
        }
    }
}