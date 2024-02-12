using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis
{
    public class BattleBuildingProduction : BattleBuilding
    {
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
        float _currentProductionDelaySecond;

        BuildingProduction _buildingProduction;

        BattleCreaturePool _creaturePool;

        readonly List<BattleCreature> _producedCreatures = new();
        readonly List<BattleEntity> _playerEntitiesWithinRange = new();

        public event Action<BattleEntity> OnEntityInRange;

        public override void Initialize(Vector3 pos, Building building)
        {
            base.Initialize(pos, building);
            _buildingProduction = building as BuildingProduction;

            if (_buildingProduction == null) return;
            for (int i = 0; i < _buildingProduction.BuildingUpgrade.CurrentLevel + 1; i++)
                StarRenderers[i].sprite = Star;

            _creaturePool = GetComponent<BattleCreaturePool>();
            _creaturePool.Initialize(_buildingProduction.ProducedCreature.Prefab);

            Transform t = transform;
            _spawner = Instantiate(SpawnerPrefab, t);
            Transform spawnerT = _spawner.transform;
            spawnerT.localPosition = SpawnPoint.localPosition;
            spawnerT.localRotation = transform.rotation;
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
            Color c = _buildingProduction.ProducedCreature.Element.Color.Primary;
            ProgressBarHandler.SetBorderColor(c);
            ProgressBarHandler.SetFillColor(Color.white);
            ProgressBarHandler.SetProgress(0);
            ProgressBarHandler.ShowProgressBar();

            while (_producedCreatures.Count < _buildingProduction.GetCurrentUpgrade().ProductionLimit)
            {
                SpawnHostileCreature();
                yield return ProductionDelay();
            }

            ProgressBarHandler.HideProgressBar();
            _productionCoroutine = null;
        }

        void SpawnHostileCreature()
        {
            Creature creature = Instantiate(_buildingProduction.ProducedCreature);
            _spawner.SpawnEntity(creature, _creaturePool.GetObjectFromPool(), 1);
            // _spawner.SpawnEntities(new List<Entity>() { creature }, portalElement: null, team: 1);
            _spawner.OnSpawnComplete += (l) =>
            {
                BattleCreature bc = l[0] as BattleCreature;

                BattleManager.AddOpponentArmyEntity(bc);
                _producedCreatures.Add(bc);

                if (bc == null) return;
                bc.InitializeHostileCreature(this);

                bc.OnDeath += (_, _) =>
                {
                    if (bc.Team == 0) return; // Team 0 creatures are resurrected
                    if (_producedCreatures.Contains(bc))
                        _producedCreatures.Remove(bc);
                    StartProductionCoroutine();
                };
            };
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out BattleEntity battleEntity)) return;
            if (battleEntity.Team == 1) return; // TODO: hardcoded team number
            battleEntity.OnDeath += RemoveEntityFromList;
            _playerEntitiesWithinRange.Add(battleEntity);
            OnEntityInRange?.Invoke(battleEntity);
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out BattleEntity battleEntity)) return;
            if (battleEntity.Team == 1) return; // TODO: hardcoded team number
            RemoveEntityFromList(battleEntity, null);
        }

        void RemoveEntityFromList(BattleEntity entity, EntityFight ignored)
        {
            entity.OnDeath -= RemoveEntityFromList;
            if (_playerEntitiesWithinRange.Contains(entity))
                _playerEntitiesWithinRange.Remove(entity);
        }

        public List<BattleEntity> GetPlayerEntitiesWithinRange()
        {
            return _playerEntitiesWithinRange;
        }
        
        IEnumerator ProductionDelay()
        {
            float totalDelay = _buildingProduction.GetCurrentUpgrade().ProductionDelay;
            _currentProductionDelaySecond = 0f;
            while (_currentProductionDelaySecond < totalDelay)
            {
                _currentProductionDelaySecond += 0.5f;
                ProgressBarHandler.SetProgress(_currentProductionDelaySecond / totalDelay);

                yield return new WaitForSeconds(0.5f);
            }
        }
        
        /* INTERACTION */
        public override void DisplayTooltip()
        {
            if (TooltipManager == null) return;

            TooltipManager.ShowTooltip(new BuildingProductionCard(_buildingProduction), gameObject);
        }
    }
}