using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class GoldMineController : BuildingController, IInteractable
    {
        [Header("Gold Mine")]
        [SerializeField] Transform _goldBagHolder;

        [SerializeField] GoldBagController[] _goldBagPrefabs;
        readonly List<GoldBagController> _goldBags = new();

        [SerializeField] Transform[] _minePoints;
        [SerializeField] Transform _emptyBagPoint;

        GoldMine _goldMine;
        public new string InteractionPrompt => "Collect gold";

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

            for (int i = 0; i < _goldMine.GetAssignedWorkers().Count; i++)
            {
                Debug.Log("Gold Mine: " + _goldMine.GetAssignedWorkers()[i].name);
                UnitCampController ucc = CampManager.SpawnUnit(_goldMine.GetAssignedWorkers()[i], transform.position);
                SetWorker(ucc);
                ucc.StartGoldMineCoroutine(_minePoints[i].position, _emptyBagPoint.position);
            }

            int goldBagsToSpawn = _goldMine.GoldAvailable / 200;
            goldBagsToSpawn = Mathf.Clamp(goldBagsToSpawn, 0, 6);
            for (int i = 0; i < goldBagsToSpawn; i++)
                SpawnGoldBag();
        }

        void SpawnGoldBag()
        {
            GoldBagController goldBag =
                Instantiate(_goldBagPrefabs[Random.Range(0, _goldBagPrefabs.Length)], _goldBagHolder);
            goldBag.transform.localPosition = new(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            goldBag.gameObject.SetActive(true);
            _goldBags.Add(goldBag);
        }

        public override bool CanInteract()
        {
            if (_goldMine.GoldAvailable <= 0) return false;
            return base.CanInteract();
        }

        public override bool Interact(Interactor interactor)
        {
            CampConsoleManager.ShowMessage($"Collected {_goldMine.GoldAvailable} gold from Gold Mine.");
            HeroCampController.Instance.DisplayFloatingText(+_goldMine.GoldAvailable + " Gold",
                GameManager.Instance.GameDatabase.GetColorByName("Gold").Primary);

            _goldMine.CollectGold();

            for (int i = _goldBags.Count - 1; i >= 0; i--)
                _goldBags[i].DestroySelf();
            _goldBags.Clear();

            return true;
        }

        public override void SetWorker(UnitCampController ucc)
        {
            base.SetWorker(ucc);
            CampConsoleManager.ShowMessage($"Unit assigned to Gold Mine.");
            ucc.StartGoldMineCoroutine(_minePoints[_goldMine.GetAssignedWorkerCount() - 1].position,
                _emptyBagPoint.position);
        }
    }
}