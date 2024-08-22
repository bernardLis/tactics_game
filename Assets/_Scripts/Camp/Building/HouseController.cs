using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero;
using Lis.Units.Peasant;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class HouseController : BuildingController, IInteractable
    {
        [Header("Blacksmith")]
        [SerializeField] Transform _standPointLeft;
        [SerializeField] Transform _standPointRight;

        [SerializeField] CrateController[] _cratePrefabs;
        [SerializeField] Transform _crateHolder;
        readonly List<CrateController> _crates = new();

        House _house;
        public new string InteractionPrompt => "Collect Peasants";

        protected override void Initialize()
        {
            Building = GameManager.Campaign.House;
            _house = (House)Building;

            base.Initialize();

            if (!_house.IsUnlocked) return;

            InitializeHouse();
        }

        protected override void Unlock()
        {
            base.Unlock();
            InitializeHouse();
        }

        void InitializeHouse()
        {
            GetComponentInChildren<UnitDropZoneController>().Initialize(this);

            for (int i = 0; i < _house.GetAssignedWorkers().Count; i++)
            {
                UnitCampController ucc = CampManager.SpawnUnit(_house.GetAssignedWorkers()[i], transform.position);
                SetWorker(ucc);
                Transform standPoint = i == 0 ? _standPointRight : _standPointLeft;
                ucc.StartHouseCoroutine(standPoint);
            }

            int cratesToSpawn = _house.AvailablePeasantCount;
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
            if (_house.AvailablePeasantCount <= 0) return false;
            return base.CanInteract();
        }

        public override bool Interact(Interactor interactor)
        {
            CampConsoleManager.ShowMessage($"Collected {_house.AvailablePeasantCount} Peasants from House.");

            for (int i = 0; i < _house.AvailablePeasantCount; i++)
            {
                HeroCampController.Instance.DisplayFloatingText("+Peasant", Color.black);
                Peasant p = Instantiate(GameManager.UnitDatabase.Peasant);
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
            CampConsoleManager.ShowMessage($"Unit assigned to House.");
            Transform standPoint = _standPointRight;
            if (_house.GetAssignedWorkers().Count > 1)
                standPoint = _standPointLeft;
            ucc.StartHouseCoroutine(standPoint);
        }
    }
}