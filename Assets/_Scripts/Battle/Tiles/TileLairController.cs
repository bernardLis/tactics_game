using System.Collections;
using System.Collections.Generic;
using Lis.Battle.Fight;
using Lis.Battle.Tiles;
using Lis.Battle.Tiles.Building;
using Lis.Units;
using Lis.Units.Creature;
using Lis.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lis
{
    public class TileLairController : TileProductionController
    {
        [SerializeField] Image _icon;
        [SerializeField] TMP_Text _productionLimitText;
        [SerializeField] TMP_Text _productionTimerText;

        PlayerUnitTracker _buildingEntityTracker;

        [SerializeField]
        protected EntitySpawner SpawnerPrefab;

        EntitySpawner _spawner;

        [SerializeField] protected Transform SpawnPoint;

        public Creature ProducedCreature;
        IEnumerator _productionCoroutine;
        int _currentProductionDelaySecond;
        CreaturePoolManager _creaturePoolManager;
        readonly List<CreatureController> _producedCreatures = new();

        protected override void OnTileEnabled(UpgradeTile upgrade)
        {
            base.OnTileEnabled(upgrade);

            _icon.sprite = upgrade.Icon;

            _creaturePoolManager = GetComponent<CreaturePoolManager>();
            _creaturePoolManager.Initialize(ProducedCreature.Prefab);
            InitializeSpawner();
            InitializePlayerEntitiesTracker();

            UpdateProductionLimitText();
        }

        protected override void OnTileUnlocked(TileController tile)
        {
            base.OnTileUnlocked(tile);
            StartProduction();
        }

        void InitializeSpawner()
        {
            Transform t = transform;
            _spawner = Instantiate(SpawnerPrefab, t);
            _spawner.OnSpawnComplete += OnCreatureSpawned;
            Transform spawnerT = _spawner.transform;
            spawnerT.localPosition = SpawnPoint.localPosition;
            spawnerT.localRotation = transform.rotation;
        }

        void InitializePlayerEntitiesTracker()
        {
            _buildingEntityTracker = GetComponentInChildren<PlayerUnitTracker>();
        }

        void StartProduction()
        {
            if (_productionCoroutine != null) return;
            _productionCoroutine = ProductionCoroutine();
            StartCoroutine(_productionCoroutine);
        }

        IEnumerator ProductionCoroutine()
        {
            yield return new WaitForSeconds(2f);

            while (_producedCreatures.Count < Upgrade.CurrentLevel + 1) // HERE: production limit`
            {
                SpawnHostileCreature();
                yield return ProductionDelay();
            }

            _productionTimerText.text = "-";
            _productionCoroutine = null;
        }

        void SpawnHostileCreature()
        {
            Creature creature = Instantiate(ProducedCreature);
            _spawner.SpawnEntity(creature, _creaturePoolManager.GetObjectFromPool(), 1);
        }

        void OnCreatureSpawned(UnitController be)
        {
            CreatureController bc = be as CreatureController;

            BattleManager.AddOpponentArmyEntity(bc);
            _producedCreatures.Add(bc);
            UpdateProductionLimitText();

            if (bc == null) return;
            bc.InitializeHostileCreature(_buildingEntityTracker);

            bc.OnDeath += (_, _) =>
            {
                if (bc.Team == 0) return; // Team 0 creatures are resurrected
                if (_producedCreatures.Contains(bc))
                    _producedCreatures.Remove(bc);
                StartProduction();
            };
        }

        public List<UnitController> GetPlayerEntitiesWithinRange()
        {
            return _buildingEntityTracker.PlayerEntitiesWithinRange;
        }

        IEnumerator ProductionDelay()
        {
            int totalDelay = Mathf.RoundToInt(20); //HERE: _upgrade.GetCurrentUpgrade().ProductionDelay
            _currentProductionDelaySecond = 0;
            while (_currentProductionDelaySecond < totalDelay)
            {
                _currentProductionDelaySecond += 1;
                int timeLeft = totalDelay - _currentProductionDelaySecond;
                int minutes = Mathf.FloorToInt(timeLeft / 60f);
                int seconds = Mathf.FloorToInt(timeLeft - minutes * 60);
                _productionTimerText.text = $"{minutes:00}:{seconds:00}";

                yield return new WaitForSeconds(1f);
            }
        }

        void UpdateProductionLimitText()
        {
            int limit = Upgrade.CurrentLevel + 1; // HERE: production limit
            _productionLimitText.text = _producedCreatures.Count + "/" + limit;
        }
    }
}