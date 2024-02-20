using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;

namespace Lis
{
    public class BattleBuildingProduction : BattleBuilding
    {
        [SerializeField] Image _icon;
        [SerializeField] TMP_Text _productionLimitText;
        [SerializeField] TMP_Text _productionTimerText;

        BattleBuildingEntityTracker _buildingEntityTracker;

        [FormerlySerializedAs("_spawnerPrefab")] [SerializeField]
        protected BattleEntitySpawner SpawnerPrefab;

        BattleEntitySpawner _spawner;

        [FormerlySerializedAs("_spawnPoint")] [SerializeField]
        protected Transform SpawnPoint;

        [FormerlySerializedAs("_starRenderers")] [SerializeField]
        protected SpriteRenderer[] StarRenderers;

        [FormerlySerializedAs("_star")] [SerializeField]
        protected Sprite Star;

        IEnumerator _productionCoroutine;
        int _currentProductionDelaySecond;
        BuildingProduction _buildingProduction;
        BattleCreaturePool _creaturePool;
        readonly List<BattleCreature> _producedCreatures = new();


        public override void Initialize(Vector3 pos, Building building)
        {
            base.Initialize(pos, building);
            _buildingProduction = building as BuildingProduction;

            if (_buildingProduction == null) return;

            _icon.sprite = _buildingProduction.ProducedCreature.Icon;

            for (int i = 0; i < _buildingProduction.BuildingUpgrade.CurrentLevel + 1; i++)
                StarRenderers[i].sprite = Star;

            _creaturePool = GetComponent<BattleCreaturePool>();
            _creaturePool.Initialize(_buildingProduction.ProducedCreature.Prefab);

            InitializeSpawner();
            InitializePlayerEntitiesTracker();

            UpdateProductionLimitText();
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
            _buildingEntityTracker = GetComponentInChildren<BattleBuildingEntityTracker>();
        }

        protected override IEnumerator ShowBuildingCoroutine()
        {
            yield return base.ShowBuildingCoroutine();
            StartProductionCoroutine();
        }

        void StartProductionCoroutine()
        {
            if (_productionCoroutine != null) return;
            _productionCoroutine = ProductionCoroutine();
            StartCoroutine(_productionCoroutine);
        }

        IEnumerator ProductionCoroutine()
        {
            yield return new WaitForSeconds(2f);

            while (_producedCreatures.Count < _buildingProduction.GetCurrentUpgrade().ProductionLimit)
            {
                SpawnHostileCreature();
                yield return ProductionDelay();
            }

            _productionTimerText.text = "-";
            _productionCoroutine = null;
        }

        void SpawnHostileCreature()
        {
            Creature creature = Instantiate(_buildingProduction.ProducedCreature);
            _spawner.SpawnEntity(creature, _creaturePool.GetObjectFromPool(), 1);
        }

        void OnCreatureSpawned(BattleEntity be)
        {
            BattleCreature bc = be as BattleCreature;

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
                StartProductionCoroutine();
            };
        }

        public List<BattleEntity> GetPlayerEntitiesWithinRange()
        {
            return _buildingEntityTracker.PlayerEntitiesWithinRange;
        }

        IEnumerator ProductionDelay()
        {
            int totalDelay = Mathf.RoundToInt(_buildingProduction.GetCurrentUpgrade().ProductionDelay);
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
            int limit = _buildingProduction.GetCurrentUpgrade().ProductionLimit;
            _productionLimitText.text = _producedCreatures.Count + "/" + limit;
        }
    }
}