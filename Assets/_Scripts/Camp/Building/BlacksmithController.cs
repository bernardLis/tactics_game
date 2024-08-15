using System.Collections.Generic;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Hero;
using Lis.Units.Hero.Items;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class BlackSmithController : BuildingController, IInteractable
    {
        [Header("Blacksmith")]
        [SerializeField] Transform _anvil;

        [SerializeField] CrateController[] _cratePrefabs;
        [SerializeField] Transform _crateHolder;
        readonly List<CrateController> _crates = new();

        Blacksmith _blacksmith;
        public new string InteractionPrompt => "Collect gold";

        protected override void Initialize()
        {
            Building = GameManager.Campaign.Blacksmith;
            _blacksmith = (Blacksmith)Building;

            base.Initialize();

            if (!_blacksmith.IsUnlocked) return;

            InitializeBlacksmith();
        }

        protected override void Unlock()
        {
            base.Unlock();
            InitializeBlacksmith();
        }

        void InitializeBlacksmith()
        {
            GetComponentInChildren<UnitDropZoneController>().Initialize(this);

            for (int i = 0; i < _blacksmith.GetAssignedWorkers().Count; i++)
            {
                UnitCampController ucc = CampManager.SpawnUnit(_blacksmith.GetAssignedWorkers()[i], transform.position);
                AssignUnit(ucc);
                ucc.StartBlacksmithCoroutine(_anvil.position);
            }

            int cratesToSpawn = _blacksmith.AvailableArmor.Count;
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
            if (_blacksmith.AvailableArmor.Count <= 0) return false;
            return base.CanInteract();
        }

        public override bool Interact(Interactor interactor)
        {
            foreach (Armor a in _blacksmith.AvailableArmor)
            {
                string n = Helpers.ParseScriptableObjectName(a.name);
                CampConsoleManager.ShowMessage($"Collected {n} from Blacksmith.");
                HeroCampController.Instance.DisplayFloatingText("+" + n, Color.black);
            }

            _blacksmith.CollectArmor();

            for (int i = _crates.Count - 1; i >= 0; i--)
                _crates[i].DestroySelf();
            _crates.Clear();

            return true;
        }

        protected override void SetWorker(UnitCampController ucc)
        {
            base.SetWorker(ucc);
            CampConsoleManager.ShowMessage($"Unit assigned to Blacksmith.");
            _blacksmith.AssignWorker(ucc.Unit);
            ucc.StartBlacksmithCoroutine(_anvil.position);
        }
    }
}